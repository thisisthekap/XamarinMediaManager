﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MediaManager.Media;

namespace MediaManager.Queue
{
    public class MediaQueue : ObservableCollection<IMediaItem>, IMediaQueue
    {
        IMediaManager MediaManager = CrossMediaManager.Current;

        public event QueueEndedEventHandler QueueEnded;

        public event QueueChangedEventHandler QueueChanged;

        public bool HasNext() => ShuffleMode == ShuffleMode.All ? _shuffledIndexes.Count() > _indexOfCurrentItemInShuffledIndexes + 1 : Count > CurrentIndex + 1;

        public IMediaItem NextItem
        {
            get
            {
                if (HasNext())
                {
                    if (ShuffleMode == ShuffleMode.All)
                    {
                        CurrentIndex = _shuffledIndexes[_indexOfCurrentItemInShuffledIndexes + 1];
                    }
                    else
                    {
                        CurrentIndex++;
                    }
                    return Current;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool HasPrevious()
        {
            if (ShuffleMode == ShuffleMode.All)
            {
                return _indexOfCurrentItemInShuffledIndexes > 0;
            }
            else
            {
                return CurrentIndex > 0;
            }
        }
        public IMediaItem PreviousItem
        {
            get
            {
                if (HasPrevious())
                {
                    if (ShuffleMode == ShuffleMode.All)
                    {
                        CurrentIndex = _shuffledIndexes[_indexOfCurrentItemInShuffledIndexes - 1];
                    }
                    else
                    {
                        CurrentIndex--;
                    }
                    return Current;
                }
                else
                {
                    return null;
                }
            }
        }


        public bool HasCurrent() => Count >= CurrentIndex;
        public IMediaItem Current => Count > 0 ? this[CurrentIndex] : null;

        private int _currentIndex = 0;
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                if (_currentIndex != value)
                    OnQueueChanged(this, new QueueChangedEventArgs());
                _currentIndex = value;
            }
        }

        public string Title { get; set; }


        private ShuffleMode _shuffleMode;
        private IList<int> _shuffledIndexes;
        private int _indexOfCurrentItemInShuffledIndexes => _shuffledIndexes.Select((v, i) => new { originalIndex = v, index = i }).First(x => x.originalIndex == CurrentIndex).index;

        public ShuffleMode ShuffleMode
        {
            get
            {
                return _shuffleMode;
            }
            set
            {
                _shuffleMode = value;
                if (ShuffleMode == ShuffleMode.All)
                {
                    // Create a shuffled remainder of the queue
                    CreateShuffledIndexes();
                    CollectionChanged += (s, e) => CreateShuffledIndexes();
                }
                else
                {
                    CollectionChanged -= (s, e) => CreateShuffledIndexes();
                }
            }
        }

        private void CreateShuffledIndexes()
        {
            Random rand = new Random();
            var ints = Enumerable.Range(CurrentIndex + 1, Count - 1)
                .Select(i => new Tuple<int, int>(rand.Next(Count), i))
                .OrderBy(i => i.Item1)
                .Select(i => i.Item2)
                .ToList();
            // We always put the current index at the start of the list
            ints.Insert(0, CurrentIndex);
            _shuffledIndexes = ints;
        }

        internal void OnQueueEnded(object s, QueueEndedEventArgs e) => QueueEnded?.Invoke(s, e);

        internal void OnQueueChanged(object s, QueueChangedEventArgs e) => QueueChanged?.Invoke(s, e);
    }
}
