const express = require("express");
const handlebars = require("express-handlebars");
const app = express();

app.engine("handlebars", handlebars.engine());
app.set("view engine", "handlebars");
app.set("views", __dirname + "/views");

app.get("/", (req, res) =>{
    res.render("home", {title: "테스트 게시판", message: "만나서 반갑습니다!"})
});

app.listen(3000);