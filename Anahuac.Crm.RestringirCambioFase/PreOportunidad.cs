using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Anahuac.Crm.RestringirCambioFase.DataLayer;

namespace Anahuac.Crm.RestringirCambioFase
{
    public class PreOportunidad : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                ServerConnection cnx = new ServerConnection(serviceProvider);

                cnx.trace.Trace("iniciamos plug in ");
                Entity entity = cnx.context.InputParameters["Target"] as Entity;
                Entity preEntity = cnx.context.PreEntityImages["PreImageP"];

                if (!ValidacionContexto(entity, preEntity, cnx))
                    return;

                CrmRepository crmRepository = new CrmRepository(cnx);

                cnx.trace.Trace("Paso valdiacion validara si es administrador ");

                if (!crmRepository.EsUsuarioAdministrador(cnx.context.UserId))
                    throw new Exception("NO PUEDE CAMBIAR DE FASE: PROSPECTO");
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }


        }

        #region Métodos Privados
        private bool ValidacionContexto(Entity entity, Entity PreEntity, ServerConnection cnx)
        {
            if (entity.LogicalName != "lead")
                return false;

            if (cnx.context.MessageName != "Update")
                return false;

            if (!(entity.Contains("traversedpath") && entity.Attributes["traversedpath"] != null))
                return false;

            string rutaRecorrida = entity.GetAttributeValue<string>("traversedpath");

            cnx.trace.Trace("rutaRecorrida " + rutaRecorrida);

            string rutaRecorridaPre = PreEntity.GetAttributeValue<string>("traversedpath");
            cnx.trace.Trace("rutaRecorridaPre " + rutaRecorridaPre);


            CrmRepository crmRepository = new CrmRepository(cnx);

            //if (!ValidarFase(rutaRecorrida, crmRepository, cnx))
            //    return false;
            if (ValidarFase(rutaRecorrida, rutaRecorridaPre, crmRepository, cnx))
            {
                cnx.trace.Trace("regreso true ValidarFase ");
                return false;

            }
            //    return false;

            return true;
        }

        private bool ValidarFase(string rutaRecorrida,string rutaRecorrida2, CrmRepository crmRepository, ServerConnection cnx)
        {
            string[] cadena = rutaRecorrida.Split(',');

            string[] cadena2 = rutaRecorrida2.Split(',');

            if (cadena != null && cadena.Length > 0)
            {
                if (cadena2.Length >= 3)
                    return false;

                string firsEtapa = cadena.FirstOrDefault().Trim();
                cnx.trace.Trace("firsEtapa  " + firsEtapa);

                string lastEtapa = cadena.Last().Trim();
                cnx.trace.Trace("lastEtapa  " + lastEtapa);

                if (firsEtapa == "f99b4d48-7aad-456e-864a-8e7d543f7495" && lastEtapa == "2955be08-cbd1-414d-a867-c2a83dc80b8e")
                    return true;


                //Guid faseId = new Guid(cadena.Last());
                //cnx.trace.Trace("faseId " + faseId);
                //string etapa = crmRepository.RecuperarFase(faseId);
                //cnx.trace.Trace("etapa " + rutaRecorrida);
                //if (etapa != null && etapa.ToUpper().Contains("Preuniversitario"))
                //    return true;
            }

            return false;
        }
        #endregion
    }
}
