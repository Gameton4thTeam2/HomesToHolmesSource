using HTH.DataModels;
using HTH.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace HTH.DataDependencySources
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_18
    /// 설명    : Quests Presenter.
    /// </summary>
    public class QuestsPresenter
    {
        public PendingSource pendingSource;
        public AcceptedSource acceptedSource;
        public InProgressSource inProgressSource;
        public NewPendingQuestCommand newPendingCommand;
        public AcceptPendingQuestCommand acceptPendingCommand;
        public RemovePendingCommand removePendingCommand;
        public RemoveAcceptedCommand removeAcceptedCommand;
        public RemoveInProgressCommand removeInProgressCommand;

        public QuestsPresenter()
        {
            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => QuestsPendingData.instance != null &&
                                              QuestsAcceptedData.instance != null &&
                                              QuestsInProgressData.instance != null);

                InitializeSource(QuestsPendingData.instance,
                                 QuestsAcceptedData.instance,
                                 QuestsInProgressData.instance);
            });
        }

        public virtual void InitializeSource(QuestsPendingData pendingData,
                                             QuestsAcceptedData acceptedData,
                                             QuestsInProgressData inProgressData)
        {
            pendingSource = new PendingSource(pendingData);
            pendingData.ItemAdded += (item) =>
            {
                pendingSource.Add(item);
            };
            pendingData.ItemRemoved += (item) =>
            {
                pendingSource.Remove(item);
            };

            acceptedSource = new AcceptedSource(acceptedData);
            acceptedData.ItemAdded += (item) =>
            {
                acceptedSource.Add(item);
            };
            acceptedData.ItemRemoved += (item) =>
            {
                acceptedSource.Remove(item);
            };

            inProgressSource = new InProgressSource(inProgressData);
            inProgressData.ItemAdded += (item) =>
            {
                inProgressSource.Add(item);
            };
            inProgressData.ItemRemoved += (item) =>
            {
                inProgressSource.Remove(item);
            };

            newPendingCommand = new NewPendingQuestCommand(pendingData);
            acceptPendingCommand = new AcceptPendingQuestCommand(pendingData, acceptedData);
            removePendingCommand = new RemovePendingCommand(pendingData);
            removeAcceptedCommand = new RemoveAcceptedCommand(acceptedData);
            removeInProgressCommand = new RemoveInProgressCommand(inProgressData);
        }

        #region Pending Source
        public class PendingSource : ObservableCollection<int>
        {
            public PendingSource(IEnumerable<int> copy)
            {
                foreach (var item in copy)
                {
                    Items.Add(item);
                }
            }
        }
        #endregion

        #region Accepted Source
        public class AcceptedSource : ObservableCollection<int>
        {
            public AcceptedSource(IEnumerable<int> copy)
            {
                foreach (var item in copy)
                {
                    Items.Add(item);
                }
            }
        }
        #endregion

        #region InProgress Source
        public class InProgressSource : ObservableCollection<int>
        {
            public InProgressSource(IEnumerable<int> copy)
            {
                foreach (var item in copy)
                {
                    Items.Add(item);
                }
            }
        }
        #endregion

        #region New Pending Command
        public class NewPendingQuestCommand
        {
            private QuestsPendingData _data;
            public NewPendingQuestCommand(QuestsPendingData data)
            {
                _data = data;
            }

            public virtual bool CanExecute(int item)
            {
                return true;
            }

            public virtual void Execute(int item)
            {
                _data.Add(item);
            }

            public virtual bool TryExecute(int item)
            {
                if (CanExecute(item))
                {
                    Execute(item);
                    return true;
                }
                return false;
            }
        }
        #endregion

        #region Accept Pending Command
        public class AcceptPendingQuestCommand
        {
            private QuestsPendingData _pendingData;
            private QuestsAcceptedData _acceptedData;
            public AcceptPendingQuestCommand(QuestsPendingData pendingData, QuestsAcceptedData acceptedData)
            {
                _pendingData = pendingData;
                _acceptedData = acceptedData;
            }

            public virtual bool CanExecute(int item)
            {
                return _pendingData.Contains(item);
            }

            public virtual void Execute(int item)
            {
                if (_pendingData.Remove(item))
                {
                    _acceptedData.Add(item);
                }
            }

            public virtual bool TryExecute(int item)
            {
                if (CanExecute(item))
                {
                    Execute(item);
                    return true;
                }
                return false;
            }
        }
        #endregion

        #region Remove Pending Command
        public class RemovePendingCommand
        {
            private QuestsPendingData _data;
            public RemovePendingCommand(QuestsPendingData data)
            {
                _data = data;
            }

            public virtual bool CanExecute(int item)
            {
                return true;
            }

            public virtual void Execute(int item)
            {
                _data.Remove(item);
            }

            public virtual bool TryExecute(int item)
            {
                if (CanExecute(item))
                {
                    _data.Remove(item);
                    return true;
                }
                return false;
            }
        }
        #endregion

        #region Remove Accepted Command
        public class RemoveAcceptedCommand
        {
            private QuestsAcceptedData _data;
            public RemoveAcceptedCommand(QuestsAcceptedData data)
            {
                _data = data;
            }

            public virtual bool CanExecute(int item)
            {
                return true;
            }

            public virtual void Execute(int item)
            {
                _data.Remove(item);
            }

            public virtual bool TryExecute(int item)
            {
                if (CanExecute(item))
                {
                    _data.Remove(item);
                    return true;
                }
                return false;
            }
        }
        #endregion

        #region Remove InProgress Command
        public class RemoveInProgressCommand
        {
            private QuestsInProgressData _data;
            public RemoveInProgressCommand(QuestsInProgressData data)
            {
                _data = data;
            }

            public virtual bool CanExecute(int item)
            {
                return true;
            }

            public virtual void Execute(int item)
            {
                _data.Remove(item);
            }

            public virtual bool TryExecute(int item)
            {
                if (CanExecute(item))
                {
                    _data.Remove(item);
                    return true;
                }
                return false;
            }
        }
        #endregion

    }
}
