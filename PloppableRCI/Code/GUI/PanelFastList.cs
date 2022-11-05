using UnityEngine;
using ColossalFramework.UI;

#pragma warning disable IDE1006 // Naming Styles


namespace PloppableRICO
{
    /// <summary>
    /// Fastlist for UI panels.  Original by Boogieman Sam (SamsamTS), via Find It.
    /// Code and comments are original.
    /// </summary>
    /// <typeparam name="O"></typeparam>
    /// <typeparam name="T"></typeparam>
    public interface IUIFastListItem<O, T> where T : UIComponent
    {
        T Component
        {
            get;
            set;
        }
        #region Methods to implement

        void Init();
        /// <summary>
        /// Method invoked very often, make sure it is fast
        /// Avoid doing any calculations, the data should be already processed any ready to display.
        /// </summary>
        /// <param name="data">What needs to be displayed</param>
        /// <param name="index">Item index</param>
        void Display(O data, int index);

        /// <summary>
        /// Change the style of the selected item here
        /// </summary>
        /// <param name="index">Item index</param>
        void Select(int index);
        /// <summary>
        /// Change the style of the item back from selected here
        /// </summary>
        /// <param name="index">Item index</param>
        void Deselect(int index);
        #endregion
    }

    /// <summary>
    /// This component is specifically designed the handle the display of
    /// very large amount of items in a scrollable panel while minimizing
    /// the impact on the performances.
    /// 
    /// This is a template. A derived class must be created because MonoBehavior types cannot be generic.
    /// public class MyHorizontalFastList: UIHorizontalFastList<dataType, UIFastListItemType> {}
    /// /*...*/
    /// component.AddUIComponent<MyHorizontalFastList>();
    /// </summary>
    public class UIFastList<O, I, T> : UIScrollablePanel
        where O : class
        where I : class, IUIFastListItem<O, T>, new()
        where T : UIComponent
    {
        #region Private members
        private FastList<I> m_items;
        private FastList<O> m_itemsData;

        //private Color32 m_color = new Color32(255, 255, 255, 255);
        private float m_itemWidth = -1;
        private float m_itemHeight = -1;
        private float m_pos = -1;
        private float m_stepSize = 0;
        private bool m_canSelect = false;
        //private int m_selectedDataId = -1;
        private int m_selectedId = -1;
        private bool m_lock;
        private bool m_updateContent = true;
        private UIComponent m_lastMouseEnter;
        private UIScrollbar m_scrollbar;
        private UIComponent m_leftArrow;
        private UIComponent m_rightArrow;
        #endregion

        #region Public accessors
        /// <summary>
        /// Can items be selected by clicking on them
        /// Default value is false
        /// Items can still be selected via selectedIndex
        /// </summary>
        public bool canSelect
        {
            get { return m_canSelect; }
            set
            {
                if (m_canSelect != value)
                {
                    m_canSelect = value;

                    if (m_items == null) return;
                    for (int i = 0; i < m_items.m_size; i++)
                    {
                        if (m_items.m_buffer[i].Component != null)
                        {
                            if (m_canSelect)
                                m_items.m_buffer[i].Component.eventClick += OnItemClicked;
                            else
                                m_items.m_buffer[i].Component.eventClick -= OnItemClicked;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Change the position in the list
        /// Display the data at the position in the top item.
        /// This doesn't update the list if the position stay the same
        /// Use DisplayAt for that
        /// </summary>
        public float listPosition
        {
            get { return m_pos; }
            set
            {
                if (m_itemWidth <= 0) return;
                if (m_pos != value && m_itemsData != null)
                {
                    float pos = Mathf.Max(Mathf.Min(value, m_itemsData.m_size - 1), 0);
                    m_updateContent = Mathf.FloorToInt(m_pos) != Mathf.FloorToInt(pos);
                    DisplayAt(pos);
                }
            }
        }

        public float maxListPosition
        {
            get
            {
                if (m_itemWidth <= 0) return 0f;
                return m_itemsData.m_size - width / m_itemWidth;
            }
        }

        /// <summary>
        /// This is the list of data that will be send to the IUIFastListItem.Display method
        /// Changing this list will reset the display position to 0
        /// You can also change itemsData.m_buffer and itemsData.m_size
        /// and refresh the display with DisplayAt method
        /// </summary>
        public FastList<O> itemsData
        {
            get
            {
                if (m_itemsData == null) m_itemsData = new FastList<O>();
                return m_itemsData;
            }
            set
            {
                if (m_itemsData != value)
                {
                    m_itemsData = value;
                    //m_selectedDataId = -1;
                    DisplayAt(0);
                }
            }
        }

        /// <summary>
        /// This MUST be set, it is the width in pixels of each item
        /// </summary>
        public float itemWidth
        {
            get { return m_itemWidth; }
            set
            {
                if (m_itemWidth != value)
                {
                    m_itemWidth = value;
                    CheckItems();
                }
            }
        }

        /// <summary>
        /// This MUST be set, it is the width in pixels of each item
        /// </summary>
        public float itemHeight
        {
            get { return m_itemHeight; }
            set
            {
                if (m_itemHeight != value)
                {
                    m_itemHeight = value;
                    CheckItems();
                }
            }
        }

        /// <summary>
        /// Currently selected item
        /// -1 if none selected
        /// </summary>
        /*public int selectedIndex
        {
            get { return m_selectedDataId; }
            set
            {
                if (m_itemsData == null || m_itemsData.m_size == 0)
                {
                    m_selectedDataId = -1;
                    return;
                }

                int oldId = m_selectedDataId;
                if (oldId >= m_itemsData.m_size) oldId = -1;
                m_selectedDataId = Mathf.Min(Mathf.Max(-1, value), m_itemsData.m_size - 1);

                int pos = Mathf.FloorToInt(m_pos);
                int newItemId = Mathf.Max(-1, m_selectedDataId - pos);
                if (newItemId >= m_items.m_size) newItemId = -1;

                if (newItemId >= 0 && newItemId == m_selectedId && !m_updateContent) return;

                if (m_selectedId >= 0 && m_selectedId < m_items.m_size)
                {
                    m_items.m_buffer[m_selectedId].Deselect(oldId);
                }

                m_selectedId = -1;

                if (newItemId >= 0)
                {
                    m_selectedId = newItemId;
                    m_items.m_buffer[m_selectedId].Select(m_selectedDataId);
                }

                if (eventSelectedIndexChanged != null && m_selectedDataId != oldId)
                    eventSelectedIndexChanged(this, m_selectedDataId);
            }
        }*/

        public O selectedItem
        {
            get; set;
        }

        public string template
        { get; set; }

        public bool selectOnMouseEnter
        { get; set; }

        public UIScrollbar scrollbar
        {
            get
            {
                return m_scrollbar;
            }
            set
            {
                m_scrollbar = value;

                if (m_scrollbar != null)
                {
                    m_scrollbar.eventValueChanged += (c, t) =>
                    {
                        if (m_lock || m_itemHeight <= 0) return;

                        int nbRow = /*Mathf.CeilToInt*/(int)(height / m_itemHeight);
                        int nbCol = /*Mathf.CeilToInt*/(int)(width / m_itemWidth);
                        int totalRow = Mathf.CeilToInt((itemsData.m_size - 1) / nbCol);

                        m_lock = true;

                        listPosition = nbCol * m_scrollbar.value; // * (totalRow - height / m_itemHeight) / (height - m_scrollbar.scrollSize - 1f);
                        m_lock = false;
                    };
                }
            }
        }

        public UIComponent leftArrow
        {
            get
            {
                return m_leftArrow;
            }
            set
            {
                m_leftArrow = value;

                if (m_leftArrow != null)
                {
                    m_leftArrow.eventMouseDown += (c, p) =>
                    {
                        if (!p.used)
                        {
                            p.Use();
                            listPosition--;
                        }
                    };

                    m_leftArrow.eventMouseWheel += (c, p) => OnMouseWheel(p);
                }
            }
        }

        public UIComponent rightArrow
        {
            get
            {
                return m_rightArrow;
            }
            set
            {
                m_rightArrow = value;

                if (m_rightArrow != null)
                {
                    m_rightArrow.eventMouseDown += (c, p) =>
                    {
                        if (!p.used)
                        {
                            p.Use();
                            listPosition++;
                        }
                    };

                    m_rightArrow.eventMouseWheel += (c, p) => OnMouseWheel(p);
                }
            }
        }

        /// <summary>
        /// The number of pixels moved at each scroll step
        /// When set to 0 or less, itemWidth is used instead.
        /// </summary>
        public float stepSize
        {
            get { return (m_stepSize > 0) ? m_stepSize : 1f; }
            set { m_stepSize = value; }
        }
        #endregion

        #region Events
        /// <summary>
        /// Called when the currently selected item changed
        /// </summary>
        //public event PropertyChangedEventHandler<int> eventSelectedIndexChanged;
        /// <summary>
        /// Called when the current display position is changed
        /// </summary>
        public event PropertyChangedEventHandler<int> eventListPositionChanged;
        #endregion

        #region Public methods
        /// <summary>
        /// Clear the list
        /// </summary>
        public void Clear()
        {
            itemsData.Clear();
            selectedItem = null;

            if (m_items != null)
            {
                for (int i = 0; i < m_items.m_size; i++)
                {
                    if (m_items.m_buffer[i].Component == null)
                    {
                        m_items.m_buffer[i].Component = CreateItem();
                        m_items.m_buffer[i].Init();
                    }
                    m_items.m_buffer[i].Component.enabled = false;
                }
            }

            CheckItems();
        }

        /// <summary>
        /// Display the data at the position in the top item.
        /// This update the list even if the position remind the same
        /// </summary>
        /// <param name="pos">Index position in the list</param>
        public void DisplayAt(float pos)
        {
            // Force autolayout off, just in case something else has set it.
            autoLayout = false;

            if (m_itemsData == null || m_itemsData.m_size == 0 || m_itemWidth <= 0 || m_itemHeight <= 0) return;

            m_selectedId = -1;

            int nbRow = /*Mathf.CeilToInt*/(int)(height / m_itemHeight);

            if (nbRow > 1)
            {
                int nbCol = /*Mathf.CeilToInt*/(int)(width / m_itemWidth);

                float maxPos = (Mathf.Ceil((itemsData.m_size - 1) / nbCol) - nbRow + 1) * nbCol;
                m_pos = Mathf.Max(Mathf.Min(pos, maxPos), 0);

                //float offset = itemHeight * (m_pos - Mathf.Floor(m_pos));
                int startPos = (Mathf.FloorToInt(m_pos) / nbCol) * nbCol;

                for (int i = 0; i < nbRow; i++)
                {
                    for (int j = 0; j < nbCol; j++)
                    {
                        int itemPos = i * nbCol + j;
                        I item = m_items.m_buffer[itemPos];
                        if (item.Component == null)
                        {
                            item.Component = CreateItem();
                            item.Init();

                            if (m_canSelect)
                                item.Component.eventClick += OnItemClicked;
                        }

                        int dataPos = startPos + itemPos;
                        if (dataPos < m_itemsData.m_size)
                        {
                            if (m_updateContent)
                                item.Display(m_itemsData[dataPos], dataPos);

                            if (m_updateContent && m_itemsData[dataPos] == selectedItem)
                            {
                                m_items.m_buffer[itemPos].Select(dataPos);
                                m_selectedId = itemPos;
                            }

                            item.Component.enabled = true;
                        }
                        else
                        {
                            item.Component.enabled = false;
                        }
                        item.Component.relativePosition = new Vector3(j * itemWidth, i * itemHeight /*- offset*/);
                    }
                }

                if (m_leftArrow != null)
                {
                    m_leftArrow.isVisible = false;
                }

                if (m_rightArrow != null)
                {
                    m_rightArrow.isVisible = false;
                }
            }
            else
            {
                float maxPos = m_itemsData.m_size - width / m_itemWidth;
                m_pos = Mathf.Max(Mathf.Min(pos, maxPos), 0);

                for (int i = 0; i < m_items.m_size; i++)
                {
                    I item = m_items.m_buffer[i];
                    if (item.Component == null)
                    {
                        item.Component = CreateItem();
                        item.Init();

                        if (m_canSelect)
                            item.Component.eventClick += OnItemClicked;
                    }

                    int dataPos = Mathf.FloorToInt(m_pos + i);
                    float offset = itemWidth * (m_pos + i - dataPos);
                    if (dataPos < m_itemsData.m_size)
                    {
                        if (m_updateContent)
                            item.Display(m_itemsData[dataPos], dataPos);

                        if (m_updateContent && m_itemsData[dataPos] == selectedItem)
                        {
                            m_items.m_buffer[i].Select(dataPos);
                            m_selectedId = i;
                        }

                        item.Component.enabled = true;
                    }
                    else
                    {
                        item.Component.enabled = false;
                    }

                    item.Component.relativePosition = new Vector3(i * itemWidth - offset, 0);
                }

                if (m_leftArrow != null)
                {
                    m_leftArrow.isEnabled = m_pos > 0f;
                    m_leftArrow.isVisible = maxPos > 0f;
                }

                if (m_rightArrow != null)
                {
                    m_rightArrow.isEnabled = m_pos < maxPos;
                    m_rightArrow.isVisible = maxPos > 0f;
                }
            }

            if (eventListPositionChanged != null && m_updateContent)
                eventListPositionChanged(this, Mathf.FloorToInt(m_pos));

            m_updateContent = true;
        }

        /// <summary>
        /// Refresh the display
        /// </summary>
        public void Refresh()
        {
            DisplayAt(m_pos);
        }

        public I GetItem(int index)
        {
            if (index < 0 || index >= m_items.m_size)
            {
                return null;
            }

            return m_items.m_buffer[index];
        }
        #endregion

        #region Overrides
        public override void Start()
        {
            base.Start();

            clipChildren = true;

            // Items
            CheckItems();
        }

        protected override void OnVisibilityChanged()
        {
            base.OnVisibilityChanged();

            if (isVisible)
            {
                height = (int)(parent.parent.height / itemHeight) * itemHeight;
            }
        }

        protected override void OnSizeChanged()
        {
            autoLayout = false;
            autoReset = false;

            base.OnSizeChanged();

            /*m_scrollbar.height = height;
            m_scrollbar.trackObject.height = height;
            m_scrollbar.AlignTo(this, UIAlignAnchor.TopRight);*/

            CheckItems();
            DisplayAt(0);
        }

        protected override void OnMouseWheel(UIMouseEventParameter p)
        {
            //base.OnMouseWheel(p);

            //if (!p.used)
            //{

            int nbRow = /*Mathf.CeilToInt*/(int)(height / m_itemHeight);
            if (nbRow > 1)
            {
                int nbCol = /*Mathf.CeilToInt*/(int)(width / m_itemWidth);

                if (m_stepSize > 0 && m_itemWidth > 0)
                    listPosition = m_pos - Mathf.Round(p.wheelDelta) * m_stepSize * nbCol / m_itemWidth;
                else
                    listPosition = m_pos - Mathf.Round(p.wheelDelta) * nbCol;
            }
            else
            {
                if (m_stepSize > 0 && m_itemWidth > 0)
                    listPosition = m_pos - Mathf.Round(p.wheelDelta) * m_stepSize / m_itemWidth;
                else
                    listPosition = m_pos - Mathf.Round(p.wheelDelta);
            }

            /*if(prevPos != listPosition)
            {
                p.Use();
            }*/

            if (selectOnMouseEnter)
                OnItemClicked(m_lastMouseEnter, p);
            //}
        }
        #endregion

        #region Private methods

        protected void OnItemClicked(UIComponent component, UIMouseEventParameter p)
        {
            if (selectOnMouseEnter) m_lastMouseEnter = component;

            int max = Mathf.Min(m_itemsData.m_size, m_items.m_size);

            for (int i = 0; i < max; i++)
            {
                if (component == (UIComponent)m_items.m_buffer[i].Component)
                {
                    int selectedIndex = i + Mathf.FloorToInt(m_pos);
                    selectedItem = m_itemsData[selectedIndex];

                    if (m_selectedId >= 0)
                    {
                        m_items.m_buffer[m_selectedId].Deselect(selectedIndex);
                    }

                    m_items.m_buffer[i].Select(selectedIndex);
                    m_selectedId = i;

                    return;
                }
            }
        }

        private void CheckItems()
        {
            if (m_itemWidth <= 0 || m_itemHeight <= 0 || template == null) return;

            int nbRow = /*Mathf.CeilToInt*/(int)(height / m_itemHeight);
            int nbCol = /*Mathf.CeilToInt*/(int)(width / m_itemWidth);

            UIScrollbar scrollbar = parent.parent.GetComponentInChildren<UIScrollbar>();
            if (scrollbar != null)
            {
                scrollbar.isVisible = nbRow > 1;
            }

            int nbItems = nbCol * nbRow;

            if (m_items == null)
            {
                m_items = new FastList<I>();
                m_items.SetCapacity(nbItems);
            }

            if (m_items.m_size < nbItems)
            {
                // Adding missing items
                for (int i = m_items.m_size; i < nbItems; i++)
                {
                    m_items.Add(new I());
                    /*m_items.Add(new I()
                        {
                            item = CreateItem()
                        });

                    m_items.m_buffer[i].Init();*/
                }
            }
            else if (m_items.m_size > nbItems)
            {
                // Remove excess items
                for (int i = nbItems; i < m_items.m_size; i++)
                {
                    if (m_items.m_buffer[i].Component != null)
                    {
                        Destroy(m_items.m_buffer[i].Component.gameObject);
                    }
                }

                m_items.SetCapacity(nbItems);
            }
        }

        private T CreateItem()
        {
            GameObject asGameObject = UITemplateManager.GetAsGameObject(template);
            T item = AttachUIComponent(asGameObject) as T;

            if (m_canSelect && !selectOnMouseEnter) item.eventClick += OnItemClicked;
            else if (m_canSelect) item.eventMouseEnter += OnItemClicked;

            return item;
        }
        #endregion
    }
}

#pragma warning restore IDE1006 // Naming Styles
