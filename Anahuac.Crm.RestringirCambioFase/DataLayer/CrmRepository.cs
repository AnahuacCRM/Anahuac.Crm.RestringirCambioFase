using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Anahuac.Crm.RestringirCambioFase.DataLayer
{
    public class CrmRepository
    {
        #region Variables Privadas -- Constructor
        private ServerConnection _cnx = default(ServerConnection);

        public CrmRepository(ServerConnection cnx)
        {
            _cnx = cnx;
        } 
        #endregion

        public bool EsUsuarioAdministrador(Guid usuarioId)
        {
            bool esAdmin = default(bool);

            QueryExpression query = new QueryExpression("systemuser");

            LinkEntity systemUserToSystemUserRoles = new LinkEntity
            {
                LinkFromEntityName = "systemuser",
                LinkToEntityName = "systemuserroles",
                LinkFromAttributeName = "systemuserid",
                LinkToAttributeName = "systemuserid",
                JoinOperator = JoinOperator.Inner
            };
            LinkEntity systemUserRolesToRole = new LinkEntity
            {
                LinkFromEntityName = "systemuserroles",
                LinkToEntityName = "role",
                LinkFromAttributeName = "roleid",
                LinkToAttributeName = "roleid",
                JoinOperator = JoinOperator.Inner
            };

            query.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, usuarioId);
            query.LinkEntities.Add(systemUserToSystemUserRoles);
            query.LinkEntities.First().LinkEntities.Add(systemUserRolesToRole);
            query.LinkEntities.First().LinkEntities.First().LinkCriteria.AddCondition("name", ConditionOperator.Equal, "administrador del sistema");

            EntityCollection coleccion = _cnx.service.RetrieveMultiple(query);

            if (coleccion != null && coleccion.Entities.Any())
                esAdmin = true;

            return esAdmin;
        }

        public string RecuperarFase(Guid faseId)
        {
            Entity entity = _cnx.service.Retrieve("processstage", faseId, new ColumnSet("stagename"));

            return entity.Contains("stagename") ? entity.GetAttributeValue<string>("stagename") : string.Empty;
        }
    }
}
