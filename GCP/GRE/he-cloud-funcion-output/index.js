require('dotenv').config();
const temaNombre = process.env.temaNombre
const credencial = process.env.credencial
const tipo       = process.env.tipo

const {PubSub} = require('@google-cloud/pubsub')
const pubsubClient = new PubSub()

exports.main = async (req, res) => {
    
    if ( req.method != 'POST' ){
        return res.status(405).send(`Método ${req.method} no permitido.`)
    }

    if ( !req.headers.authorization ){
        return res.status(401).send('Se necesita autentificación')
    }

    let key = req.headers.authorization.replace('Basic ', '')
    if ( key != credencial ){
        return res.status(401).send('No autorizado')
    }

    if ( JSON.stringify(req.body) == '{}' || req.body.trim().length === 0 ){
        return res.status(400).send(`Se debe enviar un mensaje.`)
    }
    
    const mensaje = {
        data : Buffer.from(req.body, 'utf8'),
        attributes: {
            tipo
        }
    }

    const messageId = await pubsubClient.topic(temaNombre).publishMessage(mensaje)
    const bodyResponse = { "messageId": messageId }
    return res.status(202).send(bodyResponse)
};