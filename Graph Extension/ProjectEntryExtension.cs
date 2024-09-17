using PX.Data;
using PX.Objects.PM;
using System.Collections.Generic;
using System;

namespace DPICVR
{
    public class ProjectEntryExtension : PXGraphExtension<ProjectEntry>
    {
        #region Event Handlers

        public PXAction<PMCostBudget> UpdatePercentages;
        [PXButton(DisplayOnMainToolbar = false)]
        [PXUIField(DisplayName = "Update Pecentages", Enabled = true)]
        protected virtual void updatePercentages()
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                List<CVRUpdate> cVRUpdates = new List<CVRUpdate>();

                string searchDescription = string.Empty;
                decimal? plannedToDate = 0;
                decimal? previousPlanned = 0;
                int? costCodeID = 0;
                int? projectTaskID = 0;
                decimal? percentCompleted = 0;
                bool? userSelect = false;
                bool? primarySelect = false;

                if (Base.CostBudget.Ask(message: $"Are you sure you want to update?", MessageButtons.OKCancel) != WebDialogResult.Cancel)
                {
                    PMBudgetExtension pMBudgetExt = new PMBudgetExtension();

                    foreach (PMCostBudget pMCostBudget in Base.CostBudget.Select())
                    {
                        pMBudgetExt = PXCache<PMBudget>.GetExtension<PMBudgetExtension>(pMCostBudget);

                        primarySelect = pMBudgetExt.UsrPrimary;
                        userSelect = pMBudgetExt.UsrSelect;

                        if (primarySelect == true)
                        {
                            searchDescription = pMCostBudget.Description;
                            costCodeID = pMCostBudget.CostCodeID;
                            projectTaskID = pMCostBudget.ProjectTaskID;
                            plannedToDate = pMBudgetExt.UsrDPIPlannedToDate;
                            previousPlanned = pMBudgetExt.UsrDPIPreviousPlanned;
                            percentCompleted = pMCostBudget.PercentCompleted;

                            cVRUpdates.Add(new CVRUpdate
                            {
                                Select = userSelect,
                                PreviousPlanned = previousPlanned,
                                ToDatePlanned = plannedToDate,
                                Description = searchDescription,
                                ProjectTaskID = projectTaskID,
                                CostCodeID = costCodeID,
                                PercentCompleted = percentCompleted,
                            });
                        }
                        else
                            continue;
                    }

                    foreach (var cvrUpdate in cVRUpdates)
                    {

                        foreach (PMCostBudget pMCostBudget in Base.CostBudget.Select())
                        {
                            pMBudgetExt = PXCache<PMBudget>.GetExtension<PMBudgetExtension>(pMCostBudget);
                            userSelect = pMBudgetExt.UsrSelect;

                            if (((cvrUpdate.ProjectTaskID == pMCostBudget.ProjectTaskID && cvrUpdate.CostCodeID == pMCostBudget.CostCodeID)
                                || pMCostBudget.Description.Contains(cvrUpdate.Description)) && userSelect == true)
                            {
                                pMBudgetExt.UsrDPIPlannedToDate = cvrUpdate.ToDatePlanned;
                                pMCostBudget.PercentCompleted = cvrUpdate.PercentCompleted;
                            }

                            Base.CostBudget.Update(pMCostBudget);
                        }
                    }

                    ts.Complete();

                    Base.Save.Press();
                }
            }
        }

        protected void _(Events.FieldUpdated<PMCostBudget, PMBudgetExtension.usrDPIPlannedToDate> e)
        {

            PMCostBudget newRow = e.Row;

            var pMBudgetExt = PXCache<PMBudget>.GetExtension<PMBudgetExtension>(newRow);

            if (pMBudgetExt != null)
            {
                var primarySelect = pMBudgetExt.UsrPrimary;
                var userSelect = pMBudgetExt.UsrSelect;

                if (primarySelect == true || userSelect == true)
                    pMBudgetExt.UsrDPIPreviousPlanned = Convert.ToDecimal(e.OldValue);
            }
        }

        protected void _(Events.FieldVerifying<PMCostBudget, PMBudgetExtension.usrDPIPlannedToDate> e)
        {
            if ((decimal)e.NewValue > 100)
            {
                // Acuminator disable once PX1051 NonLocalizableString [Justification]
                throw new PXSetPropertyException(Messages.PlannedToDateCannotExceed);
            }
            else if ((decimal)e.NewValue < 0)
            {
                // Acuminator disable once PX1051 NonLocalizableString [Justification]
                throw new PXSetPropertyException(Messages.PlannedToDateCannotNegative);
            }

        }

        protected void _(Events.FieldUpdated<PMCostBudget, PMBudgetExtension.usrPrimary> e)
        {

            var row = e.Row;

            var pMBudgetExt = PXCache<PMBudget>.GetExtension<PMBudgetExtension>(row);

            if (pMBudgetExt.UsrPrimary == true)
                pMBudgetExt.UsrSelect = false;
        }

        protected void _(Events.FieldUpdated<PMCostBudget, PMBudgetExtension.usrSelect> e)
        {

            var row = e.Row;

            var pMBudgetExt = PXCache<PMBudget>.GetExtension<PMBudgetExtension>(row);

            if (pMBudgetExt.UsrSelect == true)
            {
                pMBudgetExt.UsrPrimary = false;
            }
        }

        protected void _(Events.FieldUpdated<PMCostBudget, PMCostBudget.percentCompleted> e)
        {
            var row = e.Row;
            PMCostBudget newRow = e.Row;

            var pMBudgetExt = PXCache<PMBudget>.GetExtension<PMBudgetExtension>(newRow);

            if (pMBudgetExt != null)
            {
                var primarySelect = pMBudgetExt.UsrPrimary;
                var userSelect = pMBudgetExt.UsrSelect;

                if (primarySelect == true || userSelect == true)
                    newRow.LastPercentCompleted = Convert.ToDecimal(e.OldValue);
            }

        }

        protected void _(Events.FieldVerifying<PMCostBudget, PMCostBudget.percentCompleted> e)
        {
            if ((decimal)e.NewValue > 100)
            {
                // Acuminator disable once PX1051 NonLocalizableString [Justification]
                throw new PXSetPropertyException(Messages.PercentCompletedCannotExceed);
            }
            else if ((decimal)e.NewValue < 0)
            {

                // Acuminator disable once PX1051 NonLocalizableString [Justification]
                throw new PXSetPropertyException(Messages.PercentCompletedCannotNegative);
            }

        }


        #endregion
    }

    public class CVRUpdate
    {
        public bool? Select { get; set; }
        public decimal? PreviousPlanned { get; set; }
        public decimal? ToDatePlanned { get; set; }
        public string Description { get; set; }
        public int? ProjectTaskID { get; set; }
        public int? CostCodeID { get; set; }
        public decimal? PercentCompleted { get; set; }

    }
}
