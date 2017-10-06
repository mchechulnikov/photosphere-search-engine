﻿// Copyright (c) Vladimir Sadov. All rights reserved.
//
// This file is distributed under the MIT License. See LICENSE.md for details.

using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Jbta.SearchEngine.Vendor.NonBlocking.Counter
{
    /// <summary>
    /// Scalable 32bit counter that can be used from multiple threads.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal sealed class Counter32
    {
        private static readonly int MAX_CELL_COUNT = Environment.ProcessorCount * 2;
        private const int MAX_DRIFT = 1;

        private class Cell
        {
            [StructLayout(LayoutKind.Explicit)]
            public struct SpacedCounter
            {
                // 64 bytes - sizeof(int) - sizeof(objecHeader64)
                [FieldOffset(44)]
                public int cnt;
            }

            public SpacedCounter counter;
        }

        // spaced out counters
        private Cell[] cells;

        // default counter
        private int cnt;

        // how many cells we have
        private int cellCount;

        // delayed estimated count
        private int lastCntTicks;
        private int lastCnt;

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="Counter32"/>
        /// </summary>
        public Counter32()
        {
        }

        /// <summary>
        /// Returns the value of the counter at the time of the call.
        /// </summary>
        /// <remarks>
        /// The value may miss in-progress updates if the counter is being concurrently modified.
        /// </remarks>
        public int Value
        {
            get
            {
                var count = this.cnt;
                var cells = this.cells;

                if (cells != null)
                {
                    for (int i = 0; i < cells.Length; i++)
                    {
                        var cell = cells[i];
                        if (cell != null)
                        {
                            count += cell.counter.cnt;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                return count;
            }
        }

        /// <summary>
        /// Returns the approximate value of the counter at the time of the call.
        /// </summary>
        /// <remarks>
        /// EstimatedValue could be significantly cheaper to obtain, but may be slightly delayed.
        /// </remarks>
        public int EstimatedValue
        {
            get
            {
                if (this.cells == null)
                {
                    return this.cnt;
                }

                var curTicks = Environment.TickCount;
                // more than a millisecond passed?
                if (curTicks != lastCntTicks)
                {
                    lastCnt = Value;
                    lastCntTicks = curTicks;
                }

                return lastCnt;
            }
        }

        /// <summary>
        /// Increments the counter by 1.
        /// </summary>
        public void Increment()
        {
            Cell cell = null;

            int curCellCount = this.cellCount;
            if (curCellCount > 1 & this.cells != null)
            {
                cell = this.cells[GetIndex(curCellCount)];
            }

            var drift = increment(ref ChooseCntRef(cell, ref cnt));

            if (drift > MAX_DRIFT)
            {
                TryAddCell(curCellCount);
            }
        }

        /// <summary>
        /// Decrements the counter by 1.
        /// </summary>
        public void Decrement()
        {
            Cell cell = null;

            int curCellCount = this.cellCount;
            if (curCellCount > 1 & this.cells != null)
            {
                cell = this.cells[GetIndex(curCellCount)];
            }

            var drift = decrement(ref ChooseCntRef(cell, ref cnt));

            if (drift > MAX_DRIFT)
            {
                TryAddCell(curCellCount);
            }
        }

        /// <summary>
        /// Increments the counter by 'value'.
        /// </summary>
        public void Add(int value)
        {
            Cell cell = null;

            int curCellCount = this.cellCount;
            if (curCellCount > 1 & this.cells != null)
            {
                cell = this.cells[GetIndex(curCellCount)];
            }

            var drift = add(ref ChooseCntRef(cell, ref cnt), value);

            if (drift > MAX_DRIFT)
            {
                TryAddCell(curCellCount);
            }
        }

        private static ref int ChooseCntRef(Cell cell, ref int cnt)
        {
            if (cell == null)
            {
                return ref cnt;
            }

            return ref cell.counter.cnt;
        }

        private static int increment(ref int val)
        {
            return -val - 1 + Interlocked.Increment(ref val);
        }

        private static int add(ref int val, int inc)
        {
            return -val - inc + Interlocked.Add(ref val, inc);
        }

        private static int decrement(ref int val)
        {
            return val - 1 - Interlocked.Decrement(ref val);
        }

        private static int GetIndex(int cellCount)
        {
            return Environment.CurrentManagedThreadId % cellCount;
        }

        private void TryAddCell(int curCellCount)
        {
            if (curCellCount < MAX_CELL_COUNT)
            {
                var cells = this.cells;
                if (cells == null)
                {
                    var newCells = new Cell[MAX_CELL_COUNT];
                    cells = Interlocked.CompareExchange(ref this.cells, newCells, null) ?? newCells;
                }

                if (cells[curCellCount] == null)
                {
                    Interlocked.CompareExchange(ref cells[curCellCount], new Cell(), null);
                }

                if (this.cellCount == curCellCount)
                {
                    Interlocked.CompareExchange(ref this.cellCount, curCellCount + 1, curCellCount);
                    //if (Interlocked.CompareExchange(ref this.cellCount, curCellCount + 1, curCellCount) == curCellCount)
                    //{
                    //    System.Console.WriteLine(curCellCount + 1);
                    //}
                }
            }
        }
    }
}
