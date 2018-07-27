
namespace RS.Data
{
    public class OrderItem
    {
        /// <summary>
        /// 排序的字段名
        /// </summary>
        public string FieldName
        {
            get;
            set;
        } = "";
        /// <summary>
        /// 排序方式
        /// </summary>
        public SortingMode SortingMode
        {
            get;
            set;
        } = SortingMode.ASC;
    }
    /// <summary>
    /// 排序模式
    /// </summary>
    public enum SortingMode
    {
        ASC,
        DESC
    }
}
