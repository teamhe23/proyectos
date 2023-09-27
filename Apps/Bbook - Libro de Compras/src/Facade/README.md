# Descripcion del repositorio
Apis rest que integra bbook con pmm

# Consideraciones de Despliegue PRD/QA
- Ambiente de PRD -> RANCHER -> Cluste: Production -> Project : Pruebas Logisticas TP/HP -> Namespace: bbook-integracion -> Workload : integracion-bbook-api-prod
- Ambiente de QA  -> RANCHER -> Cluste: DesarrolloV2 -> Project : Pruebas Logisticas TP/HP -> Namespace: bbook-integracion -> Workload : integracion-bbook-api

mapearlo por imagen docker

# Consideraciones de Desarrollo
Developers: para desarrollos y pruebas en nuevos requerimientos o incidencencias, considerar le siguiente:
1. Tu aplicacion debe considerar lectura de variables de entorno
2. Crear un archivo "appsettings.json" en el proyecto para desarrollos/pruebas locales, con la siguiente estructura:


```json 
{ 
  "Bbook": {
    "url": "https://testbackend.bbook.cl/api/integrations/XXXXX",
    "token": "XXXXXX"
  },
  "DB": {
    "DataSource": "10.20.11.21:1525/PMM",
    "UserID": "tiendas_adm",
    "Password": "XXXX"
  }
}
```