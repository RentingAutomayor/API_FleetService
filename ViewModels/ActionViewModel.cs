using DAO_FleetService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
    public class ActionViewModel
    {
        public int act_id { get; set; }
        public string act_name { get; set; }
        public string act_description { get; set; }

		public static Actions PrepareActionToInsertDB(ActionViewModel pAction)
		{


			if (ActionViewModel.ActionExistInDB(pAction.act_name.Trim()))
			{
				throw new Exception("La Accion que intenta ingresar, ya se encuentra almacenado en la base de datos");
			}

            Actions actionToInsert = ActionViewModel.SetDataToAction(pAction);

			return actionToInsert;
		}

        private static void ActionsHasError(ActionViewModel pAction)
        {
            if (pAction.act_name.Trim() == "")
            {
                throw new Exception("El nombre de la accion no es válido");
            }
        }


        private static Actions SetDataToAction(ActionViewModel pAction)
        {

            ActionViewModel.ActionsHasError(pAction);

            Actions actionToInsert = new Actions();
            actionToInsert.act_name = pAction.act_name;
            actionToInsert.act_description = pAction.act_description;
            actionToInsert.activo = true;

            return actionToInsert;
        }


        private static bool ActionExistInDB(string pAction_name)
        {
            try
            {
                bool rta = false;
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var actionDb = db.Actions.Where(ac => ac.act_name == pAction_name && ac.activo == true).FirstOrDefault();
                    if (actionDb != null)
                    {
                        rta = true;
                    }
                    return rta;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public static bool InsertIntoDB(Actions action)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    db.Actions.Add(action);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }


}