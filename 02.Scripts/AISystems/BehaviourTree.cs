using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace HTH.AISystems
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_02_01
    /// 설명    : 행동트리
    /// </summary>
    public class BehaviourTree
    {
        public bool enabled => _enabled;
        private bool _enabled;
        private Behaviour _root;
        private volatile bool _isRunning;


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public async void Run()
        {
            if (enabled == false)
                return;

            if (_isRunning)
                return;

            _isRunning = true;
            Behaviour.Result result = await Tick();
            _isRunning = false;        
        }

        public async UniTask<Behaviour.Result> Tick()
        {
            return await _root.Invoke();
        }

        #region Builder
        private Stack<Composite> _compositeStack;
        private Behaviour _current;

        public BehaviourTree StartBuild()
        {
            _current = _root = new Root();
            _compositeStack = new Stack<Composite>();
            _enabled = true;
            return this;
        }

        public void AttachAsChild(Behaviour parent, Behaviour child)
        {
            if (parent is Composite)
            {
                ((Composite)parent).children.Add(child);
            }
            else if (parent is IChild)
            {
                ((IChild)parent).child = child;
            }
            else
            {
                throw new Exception($"[BehaviourTree] : Failed to attach {child} as child. {parent} doesn't have child.");
            }
        }

        /// <summary>
        /// 현재 Composite 에 대한 빌드를 끝내고 상위 Composite 로 돌아감.
        /// </summary>
        public BehaviourTree ExitCurrentComposite()
        {
            if (_compositeStack.Count > 1)
            {
                _compositeStack.Pop();
                _current = _compositeStack.Peek();
            }
            else if (_compositeStack.Count == 1)
            {
                _compositeStack.Pop();
                _current = null;
            }
            else
            {
                throw new Exception($"[BehaviourTree] : Failed to Exit composite. Composite stack is empty.");
            }
            return this;
        }

        public BehaviourTree Condition(Func<bool> func)
        {
            Behaviour condition = new Condition(func);
            AttachAsChild(_current, condition);
            _current = condition;            
            return this;
        }

        public BehaviourTree Execution(Func<Behaviour.Result> execute)
        {
            Behaviour execution = new Execution(execute);
            AttachAsChild(_current, execution);

            if (_compositeStack.Count > 0)
                _current = _compositeStack.Peek();
            else
                _current = null;

            return this;
        }

        public BehaviourTree Success()
        {
            Behaviour success = new Success();
            AttachAsChild(_current, success);

            if (_compositeStack.Count > 0)
                _current = _compositeStack.Peek();
            else
                _current = null;

            return this;
        }

        public BehaviourTree Failure()
        {
            Behaviour failure = new Failure();
            AttachAsChild(_current, failure);

            if (_compositeStack.Count > 0)
                _current = _compositeStack.Peek();
            else
                _current = null;

            return this;
        }

        public BehaviourTree Selector()
        {
            Composite selector = new Selector();
            AttachAsChild(_current, selector);
            _compositeStack.Push(selector);
            _current = selector;
            return this;
        }

        public BehaviourTree RandomSelector()
        {
            Composite randomSelector = new RandomSelector();
            AttachAsChild(_current, randomSelector);
            _compositeStack.Push(randomSelector);
            _current = randomSelector;
            return this;
        }

        public BehaviourTree Sequence()
        {
            Composite sequence = new Sequence();
            AttachAsChild(_current, sequence);
            _compositeStack.Push(sequence);
            _current = sequence;
            return this;
        }

        public BehaviourTree RandomSequence()
        {
            Composite randomSequence = new RandomSequence();
            AttachAsChild(_current, randomSequence);
            _compositeStack.Push(randomSequence);
            _current = randomSequence;
            return this;
        }

        public BehaviourTree Parallel(int successPolicy)
        {
            Composite parallel = new Parallel(successPolicy);
            AttachAsChild(_current, parallel);
            _compositeStack.Push(parallel);
            _current = parallel;
            return this;
        }

        public BehaviourTree QuestHistoryCondition(int questIDRequired, Rank rankRequired)
        {
            Behaviour questHistoryCondition = new QuestHistroyCondition(questIDRequired, rankRequired);
            AttachAsChild(_current, questHistoryCondition);
            _current = questHistoryCondition;
            return this;
        }

        public BehaviourTree QuestEventGenerator(int questID)
        {
            Behaviour questEventGenerator = new QuestEventGenerator(questID);
            AttachAsChild(_current, questEventGenerator);
            _current = questEventGenerator;
            return this;
        }

        #endregion
    }
}