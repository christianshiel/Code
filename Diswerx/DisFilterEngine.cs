using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDis.Dis1998;
using System.Drawing;

namespace Diswerx
{
    public abstract class DisFilter
    {
        /// <summary>
        /// Query excluded from result
        /// </summary>
        /// <returns></returns>
        public abstract List<Pdu> FilterOut();

        /// <summary>
        /// Query included within result
        /// </summary>
        /// <returns></returns>
        public abstract List<Pdu> FilterIn();

        public abstract bool PassesFilter(Pdu pdu);

        /// <summary>
        /// Set a list of pdu entities to observe
        /// </summary>
        /// <param name="pdus"></param>
        public abstract void SetObservableEntities(List<Pdu> pdus);
    }

    #region Filters

    // todo
    //public class SpatialFilter : DisFilter
    //{
    //}

    public class PduTypeFilter : DisFilter
    {
        private List<Pdu> pdus;

        private OpenDis.Enumerations.PduType pduType;

        public PduTypeFilter(OpenDis.Enumerations.PduType pduType)
        {
            this.pduType = pduType;
        }

        public override List<Pdu> FilterOut()
        {
            List<Pdu> result = new List<Pdu>();

            foreach (Pdu pdu in pdus)
            {
                if (!PassesFilter(pdu))
                {
                    result.Add(pdu);
                }
            }

            return result;
        }

        public override List<Pdu> FilterIn()
        {
            List<Pdu> result = new List<Pdu>();

            foreach (Pdu pdu in pdus)
            {
                if (PassesFilter(pdu))
                {
                    result.Add(pdu);
                }
            }

            return result;
        }

        public override void SetObservableEntities(List<Pdu> pdus)
        {
            this.pdus = pdus;
        } 

        public override bool  PassesFilter(Pdu pdu)
        {
            if (DisHelpers.GetPduType(pdu.PduType) == this.pduType)
            {
                return true;
            }

            return false;
        }
    }

    public class EntityTypeFilter : DisFilter
    {
        private List<Pdu> pdus;
        private string entityEnum;
        private EntityType entityType;

        public EntityTypeFilter(string entityEnum)
        {
            this.entityEnum = entityEnum;
            entityType = DisHelpers.ToEntityType(entityEnum);
        }

        public override List<Pdu> FilterOut()
        {
            List<Pdu> result = new List<Pdu>();

            foreach (Pdu pdu in pdus)
            {
                if (!PassesFilter(pdu))
                {
                    result.Add(pdu);
                }
            }

            return result;
        }

        public override List<Pdu> FilterIn()
        {
            List<Pdu> result = new List<Pdu>();

            foreach (Pdu pdu in pdus)
            {
                if (DisHelpers.GetPduType(pdu.PduType) == OpenDis.Enumerations.PduType.EntityState)
                {
                    if (PassesFilter(pdu))
                    {
                        result.Add(pdu);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Must be EntityState Pdus
        /// </summary>
        /// <param name="pdus"></param>
        public override void SetObservableEntities(List<Pdu> pdus)
        {
            this.pdus = pdus;
        } 

        public override bool PassesFilter(Pdu pdu)
        {
            if (DisHelpers.GetPduType(pdu.PduType) == OpenDis.Enumerations.PduType.EntityState)
            {
                if (((EntityStatePdu)pdu).EntityType == this.entityType)
                {
                    return true;
                }
            }

            return false;
        }
    }

    #endregion

    /// <summary>
    /// DIS filter engine.
    /// </summary>
    public class DisFilterEngine
    {
        private List<DisFilter> filters;

        public DisFilterEngine()
        {
            filters = new List<DisFilter>();
        }

        public void AddFilter(DisFilter filter)
        {
            filters.Add(filter);
        }

        public void RemoveFilter(DisFilter filter)
        {
            filters.Remove(filter);
        }

        public void ClearFilters()
        {
            filters.Clear();
        }
        
        public bool PassesFilters(Pdu pdu)
        {
            if (filters.Count <= 0) return true;

            foreach (DisFilter df in filters)
            {
                if (!df.PassesFilter(pdu))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Not implemented yet.
        /// </summary>
        /// <returns></returns>
        public List<Pdu> RunFilters()
        {
            List<Pdu> result = new List<Pdu>();

            //foreach (

            return result;
        }
    }
}
