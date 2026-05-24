'use strict';

const express = require('express');
const app = express();
const http = require('http');
const serv = http.Server(app);
const port = process.env.PORT || 2222;

app.get('/', (req, res) => {
	res.sendFile(__dirname + `/WebBuild/index.html`);
});

app.use('/', express.static(__dirname + '/WebBuild') );

serv.listen(port);
console.log('Server is listening on ' + port);
