using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.ViewModel
{
    public abstract class HostViewModel : ViewModelBase
    {
        private readonly Dictionary<Type, Func<ViewModelBase>> _childrenMap = new Dictionary<Type, Func<ViewModelBase>>();
        private List<ViewModelBase> _childrenList = new List<ViewModelBase>();

        private ViewModelBase _selectedChild;


        public ViewModelBase SelectedChild
        {
            get { return _selectedChild; }
            set
            {
                if (value.PageIndex != Pages.Other)
                {
                    if (_selectedChild != null && _selectedChild.PageIndex == value.PageIndex)
                        return;
                }

                SetPropertyValue(ref _selectedChild, value);
            }
        }

        protected void RegisterChild<T>(Func<T> getter) where T : ViewModelBase
        {
            Contract.Requires(getter != null);

            if (_childrenMap.ContainsKey(typeof(T)))
                return;

            _childrenMap.Add(typeof(T), getter);
        }

        /// <summary>
        /// 添加视图
        /// </summary>
        /// <param name="viewModel"></param>
        protected void AddChild(ViewModelBase viewModel)
        {
            _childrenList.Add(viewModel);
        }

        protected ViewModelBase GetChild(Type type)
        {
            Contract.Requires(type != null);
            if (_childrenMap.ContainsKey(type) == false)
                throw new InvalidOperationException("Can't resolve type " + type.ToString());

            var viewModel = _childrenMap[type];
            return viewModel();
        }

        /// <summary>
        /// 根据索引获取数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected ViewModelBase GetChildAt(int index)
        {
            return _childrenList[index];
        }

        /// <summary>
        /// 获取视图数量
        /// </summary>
        /// <returns></returns>
        protected int GetCount()
        {
            return _childrenList.Count();
        }
    }
}
