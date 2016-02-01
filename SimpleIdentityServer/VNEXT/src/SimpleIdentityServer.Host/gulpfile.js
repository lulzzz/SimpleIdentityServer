﻿/// <binding Clean='clean' />
"use strict";
var gulp = require("gulp"),
  rimraf = require("rimraf"),
  concat = require("gulp-concat"),
  cssmin = require("gulp-cssmin"),
  uglify = require("gulp-uglify"),
  project = require("./project.json");

var paths = {
    webroot: "./wwwroot/",
};

paths.js = [
    paths.webroot + "lib/jquery/dist/jquery.js",
    paths.webroot + "lib/bootstrap/dist/js/bootstrap.js"
];
paths.css = [
    paths.webroot + "lib/bootstrap/dist/css/bootstrap.css",
    paths.webroot + "lib/bootstrap/dist/css/bootstrap-theme.css",
    paths.webroot + "css/*.css"
];
paths.concatJsDest = paths.webroot + "output/site.min.js";
paths.concatCssDest = paths.webroot + "output/site.min.css";

gulp.task("clean:js", function(cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function(cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);

gulp.task("min:js", function() {    
  gulp.src(paths.js, {
      base: "."
    })
    .pipe(concat(paths.concatJsDest))
    .pipe(uglify())
    .pipe(gulp.dest("."));
});

gulp.task("min:css", function() {
  gulp.src(paths.css)
    .pipe(concat(paths.concatCssDest))
    .pipe(cssmin())
    .pipe(gulp.dest("."));
});

gulp.task("min", ["min:js", "min:css"]);
