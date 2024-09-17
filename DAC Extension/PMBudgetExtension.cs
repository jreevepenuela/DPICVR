using PX.Data;
using PX.Objects.PM;
using System;

namespace DPICVR
{
    public class PMBudgetExtension : PXCacheExtension<PMBudget>
    {
        [PXDBBool]
        [PXUIField(DisplayName = "Primary")]
        public bool? UsrPrimary { get; set; }
        public abstract class usrPrimary : PX.Data.BQL.BqlBool.Field<usrPrimary> { }

        #region UsrSelect
        [PXDBBool]
        [PXUIField(DisplayName = "Select")]
        public bool? UsrSelect { get; set; }
        public abstract class usrSelect : PX.Data.BQL.BqlBool.Field<usrSelect> { }

        [PXDBDecimal]
        [PXUIField(DisplayName = "Contract Amount")]
        public Decimal? UsrDPIContractAmt { get; set; }
        public abstract class usrDPIContractAmt : PX.Data.BQL.BqlDecimal.Field<usrDPIContractAmt> { }
        #endregion

        #region UsrDPIPlannedToDate
        [PXDBDecimal]
        [PXUIField(DisplayName = "To Date Planned %")]
        [PXDefault(TypeCode.Decimal, "0.00", PersistingCheck = PXPersistingCheck.Nothing)]
        public Decimal? UsrDPIPlannedToDate { get; set; }
        public abstract class usrDPIPlannedToDate : PX.Data.BQL.BqlDecimal.Field<usrDPIPlannedToDate> { }
        #endregion

        #region UsrDPIPreviousPlanned
        [PXDBDecimal]
        [PXUIField(DisplayName = "Previous Planned %", IsReadOnly = true)]
        [PXDefault(TypeCode.Decimal, "0.00", PersistingCheck = PXPersistingCheck.Nothing)]
        public Decimal? UsrDPIPreviousPlanned { get; set; }
        public abstract class usrDPIPreviousPlanned : PX.Data.BQL.BqlDecimal.Field<usrDPIPreviousPlanned> { }
        #endregion
    }
}
