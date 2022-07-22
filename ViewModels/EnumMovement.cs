using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_FleetService.ViewModels
{
		public enum EnumMovement
		{
				CREACION_CUPO = 1,
				ADICION_CUPO = 2,
				CANCELACION_CUPO = 3,
				ORDEN_DE_TRABAJO = 4,
				APROBACION_ORDEN_DE_TRABAJO = 5,
				CANCELACION_ORDEN_DE_TRABAJO = 6,
				LIBERACION_DE_CUPO = 7,
				FINALIZACION_ORDEN_DE_TRABAJO = 8,
				ANULACION_ORDEN_DE_TRABAJO = 9
		}
}