using System;
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
                var array = context.Patient.StructureSets?
                        .FirstOrDefault(x => x.Id == structureSet)
                        .Structures.Where(x => x.Id.ToUpper().Contains(keyword.ToUpper()) == true).Select(x => new Struct
                        {
                            StructureId = x.Id,
                            StructureVolume = x.Volume,
                            CanModify = Helpers.CheckStructure(x),
                            IsHighRes = x.IsHighResolution
                        })
                        .ToArray();
                if (keyword.Contains("Ring"))
                {
                    Struct[] newArray = new Struct[array.Length + 1];
                    newArray[0] = new Struct
                    {
                        StructureId = "<Create new structure>",
                        CanModify = true,
                        IsHighRes = true
                    };
                    Array.Copy(array, 0, newArray, 1, array.Length);
                    return newArray;
                }
                return array;
            });

        public Task<string> GetEditableRingNameAsync(string structureSetId, string ringId) =>
            RunAsync(context => GetEditableRingName(context.Patient, structureSetId, ringId));

        public string GetEditableRingName(Patient patient, string structureSetId, string ringId)
        {
            StructureSet structureSet = patient.StructureSets.FirstOrDefault(x => x.Id == structureSetId);
            string possibleStructureId = String.Empty;

            if (structureSet.Structures.Any(x => x.Id == ringId) == false) //Ring isnt present
            {
                return structureSet.Structures.Where(x => x.Id == ringId).FirstOrDefault().Id;
            }

            for (int i = 1; i <= 5; i++)
            {
                foreach (var structure in structureSet.Structures)
                {
                    if (structure.Id == ringId + i.ToString())  //possible already present
                    {
                        possibleStructureId = ringId + (i + 1).ToString();
                        break;
                    }
                }
            }
            return possibleStructureId;
            throw new Exception("Too many uneditable rings in structure set.");
        }

        public Task AddRingAsync(string structureSetId, string ptvId, string ringInnerId, string ringMiddleId, string ringOuterId, double innerMargin, double middleMargin, double outerMargin) =>
            RunAsync(context => AddRing(context.Patient, structureSetId, ptvId, ringInnerId, ringMiddleId, ringOuterId, innerMargin, middleMargin, outerMargin));

        public void AddRing(Patient patient, string structureSetId, string ptvId, string ringInnerId, string ringMiddleId, string ringOuterId, double innerMargin, double middleMargin, double outerMargin)
        {
            StructureSet structureSet = patient.StructureSets.FirstOrDefault(x => x.Id == structureSetId);
            _planGeneration.CreateRingFromPTV(structureSet, ptvId, ringInnerId, ringMiddleId, ringOuterId, innerMargin, middleMargin, outerMargin);           
        }
    }
}
