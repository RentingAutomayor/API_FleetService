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
    public class ContractController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetContractStates()
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var lsStates = db.ContractState.Where(st => st.cntrst_state == true)
                                                            .Select(st => new ContractStateViewModel
                                                            {
                                                                id = st.cntrst_id,
                                                                name = st.cntrst_name,
                                                                description = st.cntrst_description,
                                                                state = st.cntrst_state,
                                                                registrationDate = st.cntrst_registrationDate
                                                            }).ToList();
                    return Ok(lsStates);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        public IHttpActionResult GetDiscountTypes()
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var lsDiscountTypes = db.DiscountType.Where(dt => dt.dst_state == true)
                                                                    .Select(dt => new DiscountTypeViewModel
                                                                    {
                                                                        id = dt.dst_id,
                                                                        name = dt.dst_name,
                                                                        state = dt.dst_state,
                                                                        registrationDate = dt.dst_registrationDate
                                                                    }).ToList();

                    return Ok(lsDiscountTypes);
                }

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var lsContracts = db.Contract.Where(ct => ct.cntr_state == true)
                                                            .Select(ct => new ContractViewModel
                                                            {
                                                                id = ct.cntr_id,
                                                                consecutive = ct.cntr_consecutive,
                                                                code = ct.cntr_code,
                                                                name = ct.cntr_name,
                                                                observation = ct.cntr_observation,
                                                                dealer = new DealerViewModel
                                                                {
                                                                    id = ct.deal_id,
                                                                    document = ct.Dealer.deal_document,
                                                                    name = ct.Dealer.deal_name
                                                                },
                                                                client = new ClientViewModel
                                                                {
                                                                    id = ct.cli_id,
                                                                    document = ct.Client.cli_document,
                                                                    name = ct.Client.cli_name
                                                                },
                                                                contractState = new ContractStateViewModel
                                                                {
                                                                    id = ct.cntrst_id,
                                                                    name = ct.ContractState.cntrst_name,
                                                                    description = ct.ContractState.cntrst_description
                                                                },
                                                                discountType = new DiscountTypeViewModel
                                                                {
                                                                    id = ct.dst_id,
                                                                    name = ct.DiscountType.dst_name
                                                                },
                                                                discountValue = ct.cntr_discountValue,
                                                                amountOfMaintenances = ct.cntr_amountOfMaintenances,
                                                                amountVehicles = ct.cntr_amountVehicles,
                                                                startingDate = ct.cntr_startingDate,
                                                                endingDate = ct.cntr_endingDate,
                                                                duration = ct.cntr_duration,
                                                                registrationDate = ct.cntr_registrationDate
                                                            }).OrderByDescending(ct => ct.registrationDate)
                                                            .ToList()
                                                            .Take(100);

                    return Ok(lsContracts);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult GetById(int pContract_id)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var oContract = db.Contract.Where(ct => ct.cntr_state == true && ct.cntr_id == pContract_id)
                                                            .Select(ct => new ContractViewModel
                                                            {
                                                                id = ct.cntr_id,
                                                                consecutive = ct.cntr_consecutive,
                                                                code = ct.cntr_code,
                                                                name = ct.cntr_name,
                                                                observation = ct.cntr_observation,
                                                                dealer = new DealerViewModel
                                                                {
                                                                    id = ct.deal_id,
                                                                    document = ct.Dealer.deal_document,
                                                                    name = ct.Dealer.deal_name
                                                                },
                                                                client = new ClientViewModel
                                                                {
                                                                    id = ct.cli_id,
                                                                    document = ct.Client.cli_document,
                                                                    name = ct.Client.cli_name
                                                                },
                                                                contractState = new ContractStateViewModel
                                                                {
                                                                    id = ct.cntrst_id,
                                                                    name = ct.ContractState.cntrst_name,
                                                                    description = ct.ContractState.cntrst_description
                                                                },
                                                                discountType = new DiscountTypeViewModel
                                                                {
                                                                    id = ct.dst_id,
                                                                    name = ct.DiscountType.dst_name
                                                                },
                                                                discountValue = ct.cntr_discountValue,
                                                                amountOfMaintenances = ct.cntr_amountOfMaintenances,
                                                                amountVehicles = ct.cntr_amountVehicles,
                                                                startingDate = ct.cntr_startingDate,
                                                                endingDate = ct.cntr_endingDate,
                                                                duration = ct.cntr_duration,
                                                                registrationDate = ct.cntr_registrationDate
                                                            }).FirstOrDefault();

                    oContract.lsVehicleModels = this.getLisVehicleModelsByContrat((int)oContract.id);
                    oContract.lsVehicles = this.getLisVehiclesByContrat((int)oContract.id);

                    return Ok(oContract);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        public IHttpActionResult Insert(ContractViewModel pContract)
        {
            try
            {
                var transactionType = "INSERT";
                var rta = new ResponseApiViewModel();
                Contract oContractDB = new Contract();
                this.setDataToContract(pContract, ref oContractDB, transactionType);
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    db.Contract.Add(oContractDB);
                    db.SaveChanges();

                    var contract_id = db.Contract.Where(ctr => oContractDB.cntr_code == ctr.cntr_code).FirstOrDefault().cntr_id;
                    this.setVehicleModelsToContract(contract_id, pContract.lsVehicleModels);
                    this.setVehicleToContract(contract_id, pContract.lsVehicles);

                    rta.response = true;
                    rta.message = "Se ha insertado de manera correcta el contrato " + oContractDB.cntr_code;
                    return Ok(rta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Update(ContractViewModel pContract)
        {
            try
            {
                var transactionType = "UPDATE";
                var rta = new ResponseApiViewModel();

                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {

                    Contract oContractDB = db.Contract.Where(ct => ct.cntr_id == pContract.id).FirstOrDefault();
                    this.setDataToContract(pContract, ref oContractDB, transactionType);

                    db.SaveChanges();

                    var contract_id = (int)pContract.id;
                    this.deleteVehicleModelsToContract(contract_id);
                    this.deleteVehiclesToContract(contract_id);
                    this.setVehicleModelsToContract(contract_id, pContract.lsVehicleModels);
                    this.setVehicleToContract(contract_id, pContract.lsVehicles);

                    rta.response = true;
                    rta.message = "Se ha actualizado el contrato " + oContractDB.cntr_code;
                    return Ok(rta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Delete(ContractViewModel pContract)
        {
            try
            {
                var rta = new ResponseApiViewModel();

                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {

                    Contract oContractDB = db.Contract.Where(ct => ct.cntr_id == pContract.id).FirstOrDefault();
                    oContractDB.cntr_state = false;
                    oContractDB.cntr_deleteDate = DateTime.Now;
                    db.SaveChanges();

                    var contract_id = (int)pContract.id;
                    this.deleteVehicleModelsToContract(contract_id);
                    this.deleteVehiclesToContract(contract_id);

                    rta.response = true;
                    rta.message = "Se ha eliminado el contrato " + oContractDB.cntr_code + " de la base de datos";
                    return Ok(rta);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        private void setDataToContract(ContractViewModel pContract, ref Contract contract, string transactionType)
        {

            switch (transactionType)
            {
                case "INSERT":
                    var lastConsecutive = this.getLastConsecutive();
                    contract.cntr_consecutive = lastConsecutive;
                    contract.cntr_code = "PRBA" + lastConsecutive.ToString();
                    contract.cntr_registrationDate = DateTime.Now;
                    break;
                case "UPDATE":
                    contract.cntr_consecutive = pContract.consecutive;
                    contract.cntr_code = pContract.code;
                    contract.cntr_updateDate = DateTime.Now;
                    break;
            }

            contract.cntr_name = pContract.name;
            contract.cntr_observation = pContract.observation;
            contract.deal_id = pContract.dealer.id;
            contract.cli_id = pContract.client.id;
            contract.cntrst_id = pContract.contractState.id;
            contract.dst_id = pContract.discountType.id;
            contract.cntr_discountValue = pContract.discountValue;
            contract.cntr_amountOfMaintenances = (int)pContract.amountOfMaintenances;
            contract.cntr_amountVehicles = (int)pContract.amountVehicles;
            contract.cntr_startingDate = pContract.startingDate;
            contract.cntr_endingDate = pContract.endingDate;
            contract.cntr_duration = pContract.duration;
            contract.cntr_state = true;

        }

        private void deleteVehicleModelsToContract(int contract_id)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var lsVehicleModels = db.VehicleModelsByContract.Where(vm => vm.cntr_id == contract_id).ToList();
                    foreach (var vehicleModel in lsVehicleModels)
                    {
                        db.VehicleModelsByContract.Remove(vehicleModel);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        private void deleteVehiclesToContract(int contract_id)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var lsVehicles = db.VehiclesByContract.Where(vh => vh.cntr_id == contract_id).ToList();
                    foreach (var vehicle in lsVehicles)
                    {
                        db.VehiclesByContract.Remove(vehicle);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }


        private void setVehicleModelsToContract(int contract_id, List<VehicleModelViewModel> lsVehicleModels)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    foreach (var vehicleModel in lsVehicleModels)
                    {
                        VehicleModelsByContract vehicleModelByContract = new VehicleModelsByContract();
                        vehicleModelByContract.cntr_id = contract_id;
                        vehicleModelByContract.vm_id = vehicleModel.id;

                        db.VehicleModelsByContract.Add(vehicleModelByContract);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        private void setVehicleToContract(int contract_id, List<VehicleViewModel> lsVehicles)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    foreach (var vehicle in lsVehicles)
                    {
                        VehiclesByContract vehicleByContract = new VehiclesByContract();
                        vehicleByContract.cntr_id = contract_id;
                        vehicleByContract.veh_id = vehicle.id;

                        db.VehiclesByContract.Add(vehicleByContract);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }


        private int getLastConsecutive()
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var lastContract = db.Contract.OrderByDescending(ct => ct.cntr_id).FirstOrDefault();

                    if (lastContract != null)
                    {
                        return (int)lastContract.cntr_consecutive + 1;
                    }
                    else
                    {
                        return 1;
                    }

                }

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

        private List<VehicleModelViewModel> getLisVehicleModelsByContrat(int contract_id)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {

                    var lsVehicleModels = db.VehicleModelsByContract.Where(vmBCntr => vmBCntr.cntr_id == contract_id)
                                                                    .Select(vm => new VehicleModelViewModel
                                                                    {
                                                                        id = vm.vm_id,
                                                                        shortName = vm.VehicleModel.vm_shortName,
                                                                        longName = vm.VehicleModel.vm_longName,
                                                                        type = new VehicleTypeViewModel
                                                                        {
                                                                            id = vm.VehicleModel.vt_id,
                                                                            name = vm.VehicleModel.VehicleType.vt_name
                                                                        },
                                                                        brand = new BrandViewModel
                                                                        {
                                                                            id = vm.VehicleModel.vb_id,
                                                                            name = vm.VehicleModel.VehicleBrand.vb_name
                                                                        }
                                                                    }).ToList();

                    return lsVehicleModels;
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

        private List<VehicleViewModel> getLisVehiclesByContrat(int contract_id)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var lsVehicles = db.VehiclesByContract.Where(vhBCntr => vhBCntr.cntr_id == contract_id)
                                                            .Select(vh => new VehicleViewModel
                                                            {
                                                                id = vh.veh_id,
                                                                licensePlate = vh.Vehicle.veh_licensePlate,
                                                                chasisCode = vh.Vehicle.veh_chasisCode,
                                                                vehicleState = new VehicleStateViewModel { id = vh.Vehicle.vehicleState.vs_id, name = vh.Vehicle.vehicleState.vs_name },
                                                                vehicleModel = new VehicleModelViewModel
                                                                {
                                                                    id = vh.Vehicle.vm_id,
                                                                    shortName = vh.Vehicle.VehicleModel.vm_shortName,
                                                                    longName = vh.Vehicle.VehicleModel.vm_longName,
                                                                    type = new VehicleTypeViewModel
                                                                    {
                                                                        id = vh.Vehicle.VehicleModel.vt_id,
                                                                        name = vh.Vehicle.VehicleModel.VehicleType.vt_name
                                                                    },
                                                                    brand = new BrandViewModel
                                                                    {
                                                                        id = vh.Vehicle.VehicleModel.vb_id,
                                                                        name = vh.Vehicle.VehicleModel.VehicleBrand.vb_name
                                                                    }
                                                                },
                                                                year = vh.Vehicle.veh_year,
                                                                mileage = vh.Vehicle.veh_mileage,
                                                                state = vh.Vehicle.veh_state,
                                                                registrationDate = vh.Vehicle.veh_registrationDate
                                                            }).ToList();

                    return lsVehicles;
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }


    }
}
