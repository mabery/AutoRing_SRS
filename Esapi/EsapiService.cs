using System.Linq;
using System.Threading.Tasks;
using EsapiEssentials.Plugin;
using VMS.TPS.Common.Model.API;

namespace AutoRingSRS
{
    public class EsapiService : EsapiServiceBase<PluginScriptContext>, IEsapiService
    {
        private readonly RingGeneration _planGeneration;

        public EsapiService(PluginScriptContext context) : base(context)
        {
            _planGeneration = new RingGeneration();
        }

        public Task<StructSet[]> GetStructureSetsAsync() =>
            RunAsync(context =>
            {
                return context.Patient.StructureSets?
                .Select(x => new StructSet
                {
                    CreationDate = x.HistoryDateTime,
                    ImageId = x.Image.Id,
                    StructureSetId = x.Id,
                    StructureSetIdWithCreationDate = x.Id + " - " + x.HistoryDateTime.ToString(),
                    CanModify = Helpers.CheckStructureSet(context.Patient, x)
                })
                .ToArray();
            });

        public Task<Struct[]> GetStructureIdsAsync(string structureSet, string keyword) =>
            RunAsync(context =>
            {
                return context.Patient.StructureSets?
                .FirstOrDefault(x => x.Id == structureSet)
                .Structures.Where(x => x.Id.ToUpper().Contains(keyword.ToUpper()) == true).Select(x => new Struct
                {
                    StructureId = x.Id,
                    StructureVolume = x.Volume,
                    CanModify = Helpers.CheckStructure(x)
                })
                .ToArray();
            });

        public Task AddRingAsync(string structureSetId, string ptvId, string ringInnerId, string ringMiddleId, string ringOuterId, double innerMargin, double middleMargin, double outerMargin) =>
            RunAsync(context => AddRing(context.Patient, structureSetId, ptvId, ringInnerId, ringMiddleId, ringOuterId, innerMargin, middleMargin, outerMargin));

        public void AddRing(Patient patient, string structureSetId, string ptvId, string ringInnerId, string ringMiddleId, string ringOuterId, double innerMargin, double middleMargin, double outerMargin)
        {
            StructureSet structureSet = patient.StructureSets.FirstOrDefault(x => x.Id == structureSetId);
            _planGeneration.CreateRingFromPTV(structureSet, ptvId, ringInnerId, ringMiddleId, ringOuterId, innerMargin, middleMargin, outerMargin);           
        }
    }
}