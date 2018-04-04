
namespace RS.Core
{
    public class OrderItem
    {
        public string FieldName
        {
            get;
            set;
        }
        public SortingMode SortingMode
        {
            get;
            set;
        }
    }

    public enum SortingMode
    {
        ASC,
        DESC
    }
}
