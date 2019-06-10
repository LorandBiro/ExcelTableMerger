using System.Threading.Tasks;

namespace ExcelTableMerger.Configuration
{
    public interface IConfigRepository
    {
        Config Get();

        void Set(Config config);
    }
}
