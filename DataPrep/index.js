const fs = require("fs");
const path = require("path");
const csvParser = require("csv-parser");

const basePath = "C:\\code\\cosmosDbProductCatalogue\\CosmosDbProductCatalogue.Console\\Data";

// read from categories.csv and subcategories.csv
var cats = new Promise((resolve, reject) => {
    const result = [];

    const filestream = fs.createReadStream(path.join(basePath, "categories.csv"));
    const csvparser = csvParser(["catid", "name"]);

    filestream.pipe(csvparser);
    filestream.on('error', err => reject(err));
    csvparser.on("data", data => result.push(data));
    csvparser.on("end", () => resolve(result));
});

const subcats = new Promise((resolve, reject) => {
    const result = [];

    const filestream = fs.createReadStream(path.join(basePath, "subcategories.csv"));
    const csvparser = csvParser(["catid", "catName", "subcatid", "name"]);
    
    filestream.pipe(csvparser);
    filestream.on('error', err => reject(err));
    csvparser.on("data", data => result.push(data));
    csvparser.on("end", () => resolve(result));
});

// transform into final JSON
const mapper = (arr) => {
    const output = arr.map(item => {
        let obj = {};
        obj.id = item.subcatid ? `cat9${item.subcatid}` : `cat${item.catid}`;
        obj.name = item.name;

        if (item.subcatid) {
            obj.parent = { 'id': `cat${item.catid}`, 'name': item.catName };
            obj.ancestors = [{ 'id': `cat${item.catid}`, 'name': item.catName }];
        }

        return obj;
    });

    return output;
}


Promise.all([cats, subcats])
    .then(values => values.flat())
    .then(mapper)
    .then(data => {
        const stream = fs.createWriteStream(path.join(basePath, "categories.json"), "utf-8");
        stream.write(JSON.stringify(data));
    })
    .then(console.log("COMPLETED"))
    .catch(err => {
        console.log(err);
    });

console.log("DONE");