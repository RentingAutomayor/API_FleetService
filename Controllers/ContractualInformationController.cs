using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using API_FleetService.ViewModels;
using DAO_FleetService;

namespace API_FleetService.Controllers
{
    public class ContractualInformationController : ApiController
    {
        public static ContractualInformationViewModel insertContractualInformation(ContractualInformationViewModel info) {
						try
						{
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    ContractualInformationByClient contractualInfoDB = new ContractualInformationByClient();
                    ContractualInformationController.setDataContractualInformation(info,ref contractualInfoDB, true);
                    db.ContractualInformationByClient.Add(contractualInfoDB);
                    db.SaveChanges();
                    return ContractualInformationController.getLastContractualInfoInserted();
                }
            }
						catch (Exception ex)
						{
								throw ex;
						}
              
        }

        public static ContractualInformationViewModel updateContractualInformation(ContractualInformationViewModel info)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var contractualInfoDB = db.ContractualInformationByClient
                        .Where(inf => inf.cntInf_id == info.id)
                        .FirstOrDefault();

                    if (contractualInfoDB != null)
                    {
                        ContractualInformationController.setDataContractualInformation(info, ref contractualInfoDB, false);
                    }
                    else {
                        ContractualInformationController.insertContractualInformation(info);
                    }
                    
                    
                    db.SaveChanges();
                    return ContractualInformationController.getLastContractualInfoInserted();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private static void setDataContractualInformation(ContractualInformationViewModel info,ref ContractualInformationByClient contractualInfoDB, bool isToInsert) {
						try
						{
                
                contractualInfoDB.cntInf_contractCode = info.contractCode;
                contractualInfoDB.cntInf_adminPercentage = (byte)info.adminPercentage;
                contractualInfoDB.cntInf_serviceInitDate = info.serviceInitDate;
                contractualInfoDB.cntInf_serviceEndDate = info.serviceEndDate;
                contractualInfoDB.cntInf_quotaApprovalDate = info.quotaApprovalDate;
                contractualInfoDB.cntInf_quotaEndingDate = info.quotaEndingDate;
                contractualInfoDB.qt_id = (info.quotaType != null)? info.quotaType.id:null;
                contractualInfoDB.cntInf_paymentAgreement = (short)info.paymentAgreement;
                contractualInfoDB.cli_id = info.clientId;
                contractualInfoDB.cntInf_state = true;

                if (isToInsert)
                {
                    contractualInfoDB.cntInf_registrationDate = DateTime.Now;
                }
                else {
                    contractualInfoDB.cntInf_updateDate = DateTime.Now;
                }

                
            }
						catch (Exception ex)
						{

								throw ex;
						}
           
        }

        private static ContractualInformationViewModel getLastContractualInfoInserted() {
						try
						{
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var contractualInformation = db.ContractualInformationByClient

                        .Select(cntr => new ContractualInformationViewModel
                        {
                            id = cntr.cntInf_id,
                            clientId = cntr.cli_id,
                            contractCode = cntr.cntInf_contractCode,
                            serviceInitDate = cntr.cntInf_serviceInitDate,
                            serviceEndDate = cntr.cntInf_serviceEndDate,
                            quotaApprovalDate = cntr.cntInf_quotaApprovalDate,
                            quotaEndingDate = cntr.cntInf_quotaEndingDate,
                            quotaType = (cntr.qt_id != null) ? new QuotaTypeViewModel { id = cntr.qt_id, description = cntr.QuotaType.qt_description } : null,
                            paymentAgreement = (int)cntr.cntInf_paymentAgreement,
                            adminPercentage = (cntr.cntInf_adminPercentage != null) ? (int)cntr.cntInf_adminPercentage : 0,
                            registrationDate = cntr.cntInf_registrationDate,
                            updateDate = cntr.cntInf_updateDate
                        }).OrderByDescending(cnt => cnt.id)
                        .FirstOrDefault();

                    return contractualInformation;
                }
            }
						catch (Exception ex)
						{

								throw ex;
						}
            
        }

        public static ContractualInformationViewModel getContractualInformationByClient(int clientId) {
						try
						{
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var contractualInformation = db.ContractualInformationByClient
                        .Where(cntr => cntr.cli_id == clientId && cntr.cntInf_state == true)
                        .Select(cntr => new ContractualInformationViewModel
                        {
                            id = cntr.cntInf_id,
                            clientId = cntr.cli_id,
                            contractCode = cntr.cntInf_contractCode,
                            serviceInitDate = cntr.cntInf_serviceInitDate,
                            serviceEndDate = cntr.cntInf_serviceEndDate,
                            quotaApprovalDate = cntr.cntInf_quotaApprovalDate,
                            quotaEndingDate = cntr.cntInf_quotaEndingDate,
                            quotaType = (cntr.qt_id != null) ? new QuotaTypeViewModel { id = cntr.qt_id, description = cntr.QuotaType.qt_description } : null,
                            paymentAgreement = (int)cntr.cntInf_paymentAgreement,
                            adminPercentage = (cntr.cntInf_adminPercentage != null) ? (int)cntr.cntInf_adminPercentage : 0,
                            registrationDate = cntr.cntInf_registrationDate,
                            updateDate = cntr.cntInf_updateDate
                        }).FirstOrDefault();

                    if (contractualInformation != null)
                    {
                        contractualInformation.quotaDetails = FinancialInformationController.GetFinancialInformationByClientId(clientId);
                    }

                    return contractualInformation;
                }
            }
						catch (Exception ex)
						{

								throw ex;
						}
           
        }
    }
}
