using System.Linq;
using System.Windows;
using VMS.TPS.Common.Model.API;

namespace AutoRingSRS
{
    public class RingGeneration
    {
        public void CreateRingFromPTV(StructureSet structureSet, string ptvId, string ringInnerId, string ringMiddleId, string ringOuterId, double innerMargin, double middleMargin, double outerMargin)
        {
            structureSet.Patient.BeginModifications();
            Structure ptv = structureSet.Structures.Where(x => x.Id == ptvId).FirstOrDefault();
            Structure ringInner;
            Structure ringMiddle;
            Structure ringOuter;
            if (structureSet.Structures.Any(x => x.Id == ringInnerId))
                ringInner = structureSet.Structures.Where(x => x.Id == ringInnerId).FirstOrDefault();
            else
                ringInner = structureSet.AddStructure("CONTROL", ringInnerId);  //doesnt exist yet
            if (structureSet.Structures.Any(x => x.Id == ringMiddleId))
                ringMiddle = structureSet.Structures.Where(x => x.Id == ringMiddleId).FirstOrDefault();
            else
                ringMiddle = structureSet.AddStructure("CONTROL", ringMiddleId);  //doesnt exist yet
            if (structureSet.Structures.Any(x => x.Id == ringOuterId))
                ringOuter = structureSet.Structures.Where(x => x.Id == ringOuterId).FirstOrDefault();
            else
                ringOuter = structureSet.AddStructure("CONTROL", ringOuterId);  //doesnt exist yet
            if (Helpers.CheckStructure(ringInner) && Helpers.CheckStructure(ringMiddle) && Helpers.CheckStructure(ringOuter))
            {
                ringInner.ConvertToHighResolution();
                ringMiddle.ConvertToHighResolution();
                ringOuter.ConvertToHighResolution();
            }

            ringInner.SegmentVolume = ptv.Margin(innerMargin);
            ringMiddle.SegmentVolume = ptv.Margin(middleMargin);
            ringOuter.SegmentVolume = ptv.Margin(outerMargin);

            ringInner.SegmentVolume = ringInner.Sub(ptv);
            ringMiddle.SegmentVolume = ringMiddle.Sub(ptv);
            ringOuter.SegmentVolume = ringOuter.Sub(ptv);

            ringMiddle.SegmentVolume = ringMiddle.Sub(ringInner);
            ringOuter.SegmentVolume = ringOuter.Sub(ringInner);
            ringOuter.SegmentVolume = ringOuter.Sub(ringMiddle);

            ringInner.SegmentVolume = ringInner.And(structureSet.Structures.Single(x => x.Id == "BODY"));  //clear outside of body
            ringMiddle.SegmentVolume = ringMiddle.And(structureSet.Structures.Single(x => x.Id == "BODY"));  //clear outside of body
            ringOuter.SegmentVolume = ringOuter.And(structureSet.Structures.Single(x => x.Id == "BODY"));  //clear outside of body
        }
    }
}
