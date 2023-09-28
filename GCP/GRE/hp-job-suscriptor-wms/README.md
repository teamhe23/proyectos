# Descripcion del repositorio
Job que extrae las tramas de WMS alojadas en pubsub y lo almacena en la BD

# Consideraciones de Despliegue PRD/Prueba
|Ambiente | Cluster Rancher | Projecto Rancher | Namespace | Workload |
|-----------|-----------|-----------|-----------|-----------|
|Producción| Production | GuiaRemision | hp-gre | hp-gre-job-suscriptor-wms |
|Prueba| desarrolloV2 | GuiaRemision | hp-gre | hp-gre-job-suscriptor-wms |

# Consideraciones de Desarrollo
Developers: para desarrollos y pruebas en nuevos requerimientos o incidencencias, considerar le siguiente:
1. Tu aplicacion debe considerar lectura de variables de entorno
2. Crear un archivo "appsettings.json" en la raíz del proyecto Facade para desarrollos/pruebas locales, con la siguiente estructura:

appsettings.json
```json
{
  "Oracle": {
    "DataSource": "10.20.11.95:1525/HPAX01",
    "User": "EDSR",
    "Password": "xxxxxx"
  },

  "Settigns": {
    "TiempoEsperaSegundos": "10",
    "RutaGCPCredenciales": "CredencialesGCP.json"
  },

  "PubSub": {
    "Proyecto": "clever-axe-365620",
    "Suscripcion": "wms"
  }
}
```

3. Crear un archivo "CredencialesGCP.json" con la cuenta de servicio para conectar a GCP.

CredencialesGCP.json
```json
{
  "type": "service_account",
  "project_id": "project",
  "private_key_id": "private",
  "private_key": "-----BEGIN PRIVATE KEY-----",
  "client_email": "email@xxx.gserviceaccount.com",
  "client_id": "xxxx",
  "auth_uri": "https://accounts.google.com/o/oauth2/auth",
  "token_uri": "https://oauth2.googleapis.com/token",
  "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
  "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/"
}
```
