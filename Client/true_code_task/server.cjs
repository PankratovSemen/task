var express = require('express');
var serveStatic = require('serve-static');
app = express();
app.use(serveStatic(__dirname + "/dist"));
var port = process.env.PORT || 5001;
var hostname = '0.0.0.0';

app.listen(port, hostname, () => {
    console.log(`Server running at http://${hostname}:${port}/`);
});