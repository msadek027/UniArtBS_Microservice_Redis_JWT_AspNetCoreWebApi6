namespace Workflow.WorkflowCommon
{
    public class MenuItemView
    {
        public Int64? MenuID { get; set; }
        public string MenuName { get; set; }
        public Int64? ParentID { get; set; }
        public string FormURL { get; set; }
        //public bool HasChild { get; set; }
        public IList<MenuItemView> MenuItemList { get; set; }
    }
}
