using System.Threading.Tasks;

namespace AutoRingSRS
{
    public interface IEsapiService
    {
        Task<StructSet[]> GetStructureSetsAsync();
        Task<Struct[]> GetStructureIdsAsync(string structureSet, string keyword);
        Task<string> GetEditableRingNameAsync(string structureSetId, string ringId);
        Task AddRingAsync(string structureSetId, string ptvId, string ringInnerId, string ringMiddleId, string ringOuterId, double innerMargin, double middleMargin, double outerMargin);
    }
}
