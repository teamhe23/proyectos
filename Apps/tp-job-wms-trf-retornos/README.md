# Descripcion del repositorio
Job que procesa las order de retorno

# Consideraciones de Despliegue PRD/Prueba
|Ambiente | Cluster Rancher | Projecto Rancher | Namespace | Workload |
|-----------|-----------|-----------|-----------|-----------|
|Producción| Production | Integracion WMS API | tp-wms | tp-job-wms-order-trf-retorno |
|Prueba| desarrolloV2 | Integracion WMS API | tp-wms | tp-job-wms-order-trf-retorno|

# Consideraciones de Desarrollo
Developers: para desarrollos y pruebas en nuevos requerimientos o incidencencias, considerar le siguiente:
1. Tu aplicacion debe considerar lectura de variables de entorno
2. Crear un archivo "appsettings.json" en la raíz del proyecto Facade para desarrollos/pruebas locales, con la siguiente estructura:


```json
{
  "db": {
    "oracle": {
      "DataSource": "10.20.11.167:1525/PMM",
      "User": "TIENDAS_ADM",
      "Password": "xxxx"
    }
  },
  "api": {
    "wms": {
      "Url": "https://ta15.wms.ocs.oraclecloud.com/tpsa_test/",
      "User": "tpsa_sistemas",
      "Password": "xxxx"
    }
  },
  "Settigns": {
    "TiempoEsperaSegundos": "5"
  }
}
```