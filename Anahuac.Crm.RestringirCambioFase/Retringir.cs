using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Anahuac.Crm.RestringirCambioFase.DataLayer;

namespace Anahuac.Crm.RestringirCambioFase
{
    public class Retringir : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                ServerConnection cnx = new ServerConnection(serviceProvider);
                Entity entity = cnx.context.InputParameters["Target"] as Entity;
                Entity preEntity = cnx.context.PreEntityImages["PreImage"];

                cnx.trace.Trace("antes de validaciones stepname ");
                if (!ValidacionContexto(entity, preEntity, cnx))
                    return;

                CrmRepository crmRepository = new CrmRepository(cnx);
                cnx.trace.Trace("antes de ir a ver los permisos del usuario ");
                bool esAdmin = crmRepository.EsUsuarioAdministrador(cnx.context.UserId);
                cnx.trace.Trace("validado el usuario es administrador " + esAdmin);
                if (!esAdmin)
                    throw new Exception("NO PUEDE CAMBIAR DE FASE: OPORTUNIDAD");
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        #region Métodos Privados
        private bool ValidacionContexto(Entity entity, Entity preEntity, ServerConnection cnx)
        {

            string faseEntity = "";
            string fasePreEntity = "";

            if (entity.LogicalName != "opportunity")
                return false;

            if (cnx.context.MessageName != "Update")
                //return false;





                if (!(entity.Contains("stepname") && entity.Attributes["stepname"] != null))
                    return false;

            if (!(preEntity.Contains("stepname") && preEntity.Attributes["stepname"] != null))
                return false;

            faseEntity = entity.GetAttributeValue<string>("stepname");
            fasePreEntity = preEntity.GetAttributeValue<string>("stepname");

            string onlyname = "",anlynamePreenty="";

            string[] fasename = faseEntity.Split('-');
            if (fasename != null && fasename.Length > 0)
            {
                onlyname = fasename.Last();
            }

            cnx.trace.Trace("entity:" + onlyname);

            string[] fasenamePre = fasePreEntity.Split('-');
            if (fasenamePre != null && fasenamePre.Length > 0)
            {
                anlynamePreenty = fasenamePre.Last();
            }

            cnx.trace.Trace("Preentity:" + anlynamePreenty);

            //if (faseEntity.Contains("3-Prospecto") && fasePreEntity.Contains("3-Prospecto"))
            //    return false;

            if (onlyname != "" && onlyname.Trim() == "Prospecto"&& anlynamePreenty.Trim() == "Prospecto")
                return false;



            cnx.trace.Trace("entity:" + faseEntity.ToString() + "  preentity: " + fasePreEntity.ToString());



            return true;
        }
        #endregion
    }
}
