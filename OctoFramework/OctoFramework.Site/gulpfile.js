/// <binding AfterBuild='build' ProjectOpened='default' />
"use strict"
// Include Our Plugins
let gulp = require('gulp'),
    jshint = require('gulp-jshint'),
    babel = require('gulp-babel'),
    sass = require('gulp-sass'),
    sassGlobbing = require('gulp-sass-glob'),
    concat = require('gulp-concat'),
    uglify = require('gulp-uglify'),
    rename = require('gulp-rename'),
    plumber = require('gulp-plumber'),
    notify = require('gulp-notify'),
    browsersync = require('browser-sync'),
    autoprefixer = require('gulp-autoprefixer'),
    cssmin = require('gulp-cssmin'),
    cssimport = require('gulp-cssimport'),
    webpack = require('webpack-stream'),
    webpackDevMiddleware = require('webpack-dev-middleware'),
    webpackHotMiddleware = require('webpack-hot-middleware'),
    webpackStatic = require('webpack'),
    gutil = require('gulp-util'),
    sassLint = require('gulp-sass-lint'),
    stylish = require('jshint-stylish'),
    sort = require('gulp-sort'),
    imagemin = require('gulp-imagemin'),
    webserver = require('gulp-webserver');

let webpackConfig = require('./webpack.config.js'),
    bundler = webpackStatic(webpackConfig);

gulp.task('browser-sync', function () {
    browsersync({
        open: false,
        notify: true,
        ghostMode: {
            clicks: false,
            forms: true,
            scroll: false
        },
        proxy: {
            target: 'localhost:8088',
            middleware: [
                webpackDevMiddleware(bundler, {
                    publicPath: webpackConfig.output.publicPath,
                    stats: { colors: true }
                    // http://webpack.github.io/docs/webpack-dev-middleware.html
                }),
                webpackHotMiddleware(bundler)
            ]
        }
    });
});

gulp.task('lint', function () {
    return gulp.src(['assets/js/*.js', '!assets/js/respond.min.js'])
        .pipe(jshint({
            esversion: 6
        }))
        .pipe(jshint.reporter('jshint-stylish'))
        .pipe(jshint.reporter('fail'));
});

gulp.task('sass:lint', function () {
    return gulp.src([
        '!assets/styles/scss/config/_reset.scss',
        '!assets/styles/scss/config/_variables.scss',
        '!assets/styles/scss/config/_fonts.scss',
        '!assets/styles/scss/mixins/_font-size.scss',
        '!assets/styles/scss/mixins/_rgba.scss',
        'assets/styles/scss/**/*.scss'
    ])
        .pipe(sassLint({
            options: {
                formatter: 'stylish',
                'max-warnings': 1
            },
            rules: {
                'no-ids': 2, // Severity 0 (disabled)
                'no-css-comments': 0,
                'variable-name-format': 0,
                'final-newline': 0,
                'no-important': 0,
                'no-mergeable-selectors': 1, // Severity 1 (warning)
                'pseudo-element': 0,
                'placeholder-in-extend': 0,
                'no-url-domains': 0,
                'no-url-protocols': 0,
                'mixins-before-declarations': 0,
                'property-sort-order': 0
            }
        }))
        .pipe(sassLint.format())
        .pipe(sassLint.failOnError());
});

//webserver for local dev
gulp.task('webserver', function () {
    gulp.src('.')
        .pipe(webserver({
            port: 8088,
        }));
});

// Compile Our Sass
gulp.task('sass:compile', ['sass:lint'], function () {
    return gulp.src('assets/styles/scss/**/*.scss')
        .pipe(plumber({ errorHandler: errorAlert }))
        .pipe(sassGlobbing())
        .pipe(sass())
        .pipe(autoprefixer({
            browsers: ['last 5 versions'],
            cascade: false
        }))
        .pipe(gulp.dest('assets/styles/css/'))
        .pipe(browsersync.stream())
        .pipe(cssimport())
        .pipe(cssmin({ processImport: true }))
        .pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest('assets/styles/css'));
});

gulp.task('imagemin', () =>
    gulp.src(['assets/img/**/*'])
        .pipe(imagemin())
        .pipe(gulp.dest('assets/dist/img/'))
);

gulp.task('scripts', ['lint'], function () {
    return gulp.src([
        /** DO NOT PUT EXTERNAL LIBRARIES HERE PUT THEM IN THE concatExternalScripts FUNCTION **/
        'assets/js/*.js',
        'assets/js/components/*.js'])
        .pipe(plumber({ errorHandler: errorAlert }))
        .pipe(babel({
            presets: ['es2015']
        }))
        .pipe(concat('bloc.js'))
        .pipe(gulp.dest('assets/js/dist'))
        .pipe(rename('bloc.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest('assets/js/dist'))
        .pipe(notify({ message: "Javascript linted and compiled", title: "Compilation Successful" }))
});

gulp.task('bundle:minify', function () {
    return gulp.src([
        'assets/js/dist/bundle.js'
    ])
        .pipe(gulp.dest('assets/js/dist'))
        .pipe(rename('bundle.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest('assets/js/dist'))
});

// Concatenate & Minify JS
gulp.task('webpack:build', function () {
    return gulp.src([
        'assets/js/react/**/*.js*'
    ])
        .pipe(plumber({ errorHandler: errorAlert }))
        .pipe(webpack(require('./webpack.production.config')))
        .pipe(gulp.dest('assets/js/dist'))
        .pipe(notify({ message: "React built" }))
});

gulp.task('scripts:concat-external:min', ['scripts'], concatExternalScripts(true));

gulp.task('scripts:concat-external', ['scripts'], concatExternalScripts(false));

gulp.task('fonts', function () {
    return gulp.src([
        'assets/vendor/font-awesome/fonts/*'
    ])
        .pipe(gulp.dest('assets/styles/fonts'));
});

function concatExternalScripts(min) {
    return () => {
        return gulp.src([
            /** put your libraries here **/
            'assets/vendor/jquery/dist/jquery.min.js',
            'assets/vendor/slick/slick.js',
            'assets/vendor/babel-polyfill/dist/polyfill.min.js',
            'assets/vendor/isotope/dist/isotope.pkgd.js',
            'assets/js/lib/**/*.js',
            'assets/js/dist/bloc' + (min ? '.min' : '') + '.js'
        ])
            .pipe(concat('bloc' + (min ? '.min' : '') + '.js'))
            .pipe(gulp.dest('assets/js/dist'));
    }
}

// Watch Files For Changes
gulp.task('watch', function () {
    gulp.watch('assets/js/dist/bloc.js').on('change', browsersync.reload);
    gulp.watch(['Views/**/*.cshtml']).on('change', browsersync.reload);
    gulp.watch(['assets/js/*.js', 'assets/js/components/*.js', 'assets/js/react/*.js*'], ['lint', 'scripts', 'scripts:concat-external:min', 'scripts:concat-external']);
    gulp.watch(['assets/js/react/**/*.js*'], ['webpack:build']);
    gulp.watch(['assets/js/dist/bundle.js'], ['bundle:minify']);
    gulp.watch('assets/styles/scss/**/*.scss', ['sass:lint', 'sass:compile']);//.on( 'change', browsersync.stream );
});

// Default Task
gulp.task('default', ['lint', 'sass:lint', 'sass:compile', 'scripts', 'scripts:concat-external:min', 'scripts:concat-external', 'fonts', 'watch', 'webpack:build', 'bundle:minify', 'browser-sync', 'webserver']);
gulp.task('build', ['lint', 'sass:lint', 'sass:compile', 'scripts', 'scripts:concat-external:min', 'scripts:concat-external', 'fonts', 'webpack:build', 'bundle:minify']);

function errorAlert(error) {
    console.log(error.toString());//Prints Error to Console
    this.emit("end"); //End function
}