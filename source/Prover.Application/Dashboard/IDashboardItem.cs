using System.Threading.Tasks;
using Prover.Application.Interfaces;

namespace Prover.Application.Dashboard
{
    public interface IDashboardItem
    {
        string Title { get; }
        string GroupName { get; }
        int SortOrder { get; set; }

    }
    public interface IDashboardValueViewModel : IDashboardItem
    {
        int Value { get; }
    }
}