# Descripcion del repositorio
Código para recibir los modelos de datos de la interfaces de salida de WMS.

# Consideraciones de Desarrollo
Developers: para desarrollos y pruebas en nuevos requerimientos o incidencencias, considerar le siguiente:
1. Actualizar las librerías con el comando "npm install".
2. Crear un archivo ".env" en la raíz del proyecto para desarrollos/pruebas locales, con la siguiente estructura:

.env
```text
temaNombre=he-wms-integracion
credencial=
tipo=    (verificacion_asn/output_load/order_crossdock/inventory_history
PUBSUB_EMULATOR_HOST=localhost:8085
PUBSUB_PROJECT_ID=pubsubdemo
```
