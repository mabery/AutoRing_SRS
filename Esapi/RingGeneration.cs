using System.Linq;
using VMS.TPS.Common.Model.API;

namespace AutoRingSRS
{
    public class RingGeneration
    {
        public void CreateRingFromPTV(StructureSet structureSet, string ptvId, string ringInnerId, string ringMiddleId, string ringOuterId, double innerMargin, double middleMargin, double outerMargin)
        {
            structureSet.Patient.BeginModifications();
            Structure ptv = structureSet.Structures.Where(structure => structure.Id == ptvId).FirstOrDefault();
            Structure ringInner;
            Structure ringMiddle;
            Structure ringOuter;
            try
            {
                ringInner = structureSet.AddStructure("CONTROL", ringInnerId);  //doesnt exist yet
            }
            catch
            {               
                ringInner = structureSet.Structures.FirstOrDefault(x => x.Id == ringInnerId);  //already exists 
                ringInner.SegmentVolume = ringInner.Sub(structureSet.Structures.Single(x => x.Id == "BODY"));  //cleanup
            }
            try
            {
                ringMiddle = structureSet.AddStructure("CONTROL", ringMiddleId);  //doesnt exist yet
            }
            catch
            {
                ringMiddle = structureSet.Structures.FirstOrDefault(x => x.Id == ringMiddleId);  //already exists 
                ringMiddle.SegmentVolume = ringMiddle.Sub(structureSet.Structures.Single(x => x.Id == "BODY"));  //cleanup
            }
            try
            {
                ringOuter = structureSet.AddStructure("CONTROL", ringOuterId);  //doesnt exist yet
            }
            catch
            {
                ringOuter = structureSet.Structures.FirstOrDefault(x => x.Id == ringOuterId);  //already exists 
                ringOuter.SegmentVolume = ringOuter.Sub(structureSet.Structures.Single(x => x.Id == "BODY"));  //cleanup
            }
            //try
            //{
            //    ringInner.ConvertToHighResolution();
            //    ringMiddle.ConvertToHighResolution();
            //    ringOuter.ConvertToHighResolution();
            //}
            //catch  //cannot change resolution
            //{
            //    MessageBox.Show("Cannot change structure resolution");
            //}
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
