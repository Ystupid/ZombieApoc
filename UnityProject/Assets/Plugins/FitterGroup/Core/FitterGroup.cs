using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI.FitterGroup
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ScrollRect))]
    public class FitterGroup : EditorRunnable, ILayoutable
    {
        public enum FitterAxis
        {
            Horizontal,
            Vertical,
        }

        [SerializeField] private Padding m_Padding;
        public Padding Padding
        {
            get => m_Padding;
            set
            {
                m_Padding = value;
                Refresh();
            }
        }

        [SerializeField] private Vector2 m_CellSize = new Vector2(200, 200);
        public Vector2 CellSize
        {
            get => m_CellSize;
            set
            {
                m_CellSize = value;
                Refresh();
            }
        }

        [SerializeField] private Vector2 m_Spacing = new Vector2(50, 50);
        public Vector2 CellSpacing
        {
            get => m_Spacing;
            set
            {
                m_Spacing = value;
                Refresh();
            }
        }

        private LayoutBase m_Layout;

        [SerializeField] private FitterAxis m_LayoutAxis;
        public FitterAxis LayoutAxis
        {
            get => m_LayoutAxis;
            set
            {
                m_LayoutAxis = value;
                ChangeLayout(m_LayoutAxis);
                Refresh();
            }
        }

        [SerializeField] private int m_ConstraintCount;
        public int ConstraintCount
        {
            get => m_ConstraintCount;
            set
            {
                m_ConstraintCount = Mathf.Max(1, m_ConstraintCount);
                Refresh();
            }
        }

        [SerializeField] private ScrollRect m_ScrollRect;
        public ScrollRect ScrollRect
        {
            get => m_ScrollRect;
            private set => m_ScrollRect = value;
        }

        [SerializeField] private GameObject _template;
        public GameObject Template
        {
            get => _template;
            set
            {
                _template = value;
                Refresh();
            }
        }

        private IList _dataSource;
        private Queue<RectTransform> _itemPool;
        public RectTransform TargetRect => m_ScrollRect.content;
        public int ItemCount => _dataSource == null ? 0 : _dataSource.Count;

        public void DisableItem(int index, RectTransform itemRect)
        {
            InternalReleaseItem(itemRect);
        }

        public RectTransform EnableItem(int index)
        {
            var item = InternalCreateItem();

            OnBindItem?.Invoke(index, item);

            return item;
        }

        public Action<int, RectTransform> OnBindItem;
        public Action<int, RectTransform> OnHideItem;

        public void Init(IList source, Action<int, RectTransform> bindItem)
        {
            _dataSource = source;

            _template?.SetActive(false);

            _itemPool = new Queue<RectTransform>();
            OnBindItem = bindItem;

            ChangeLayout(LayoutAxis);

            OnRectScroll(Vector2.zero);
            m_ScrollRect.onValueChanged.AddListener(OnRectScroll);
        }

        #region UseEditor
        protected override void EditorInit()
        {
            if (m_ScrollRect == null) m_ScrollRect = GetComponentInChildren<ScrollRect>();

            ChangeLayout(LayoutAxis);
        }

        protected override void EditorUpdate()
        {
            if (!Application.isPlaying)
                Tick();
        }

        protected override void EditorValidate()
        {
            ChangeLayout(LayoutAxis);
            ConstraintCount = ConstraintCount;
        }
        #endregion

        private void OnRectScroll(Vector2 rect) => Tick();

        public void Tick() => m_Layout?.Tick();
        public void Clear() => m_Layout?.Clear();
        public void Refresh() => m_Layout?.Refresh();

        public void Refresh(IList source)
        {
            _dataSource = source;

            Refresh();
        }

        /// <summary>
        /// 切换布局
        /// </summary>
        /// <param name="fitterAxis"></param>
        private void ChangeLayout(FitterAxis fitterAxis)
        {
            Clear();

            if (_dataSource != null)
                m_Layout = LayoutHelper.GetLayout(fitterAxis);
            else
                m_Layout = LayoutHelper.GetLayoutEditor(fitterAxis);

            m_Layout.Init(this);
        }

        private void InternalReleaseItem(RectTransform item)
        {
            _itemPool.Enqueue(item);

            item.gameObject.SetActive(false);
        }

        private RectTransform InternalCreateItem()
        {
            var item = default(RectTransform);

            if (_itemPool.Count > 0)
            {
                item = _itemPool.Dequeue();
            }
            else
            {
                var parent = m_ScrollRect.content;

                if (item == null)
                {
                    item = Instantiate(Template, parent).GetComponent<RectTransform>();
                }

                item.gameObject.SetActive(true);
            }

            return item;
        }
    }

    [System.Serializable]
    public struct Padding
    {
        public float Left;
        public float Right;
        public float Top;
        public float Bottom;

        public float Height => Top + Bottom;
        public float Width => Left + Right;
    }
}