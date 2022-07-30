var child_process = require("child_process");
var process = require("process");
var fsp = require("fs-extra"); //require("fs/promises");
var stream = require("stream");
var path = require("path");


module.exports = function(grunt) {

	function getShellTask(command, args = [], cwd = undefined) {
		function subTask() {
			var done = this.async();
			grunt.log.writeln("Starting " + command + " " + args.join(" "));

			if (process.platform === "win32") {
				// Beware: https://github.com/nodejs/node-v0.x-archive/issues/2318
				// To workaround we run cmd.exe as the target instead.
				var args2 = ["/S", "/C", command, ...args];
				var child = child_process.spawn("cmd.exe", args2, {
					stdio: 'inherit',
					env: {...process.env},
					cwd: cwd,
				});
			} else {
				child = child_process.spawn(command, args, {
					stdio: 'inherit',
					env: {...process.env},
					cwd: cwd,
				});
			}
			child.on('close', code => {
				if (code !== 0) done(new Error("Child process failed, exited with code " + code));
				else done();
			});
		}
		return subTask;
	}

	grunt.initConfig({
		pkg: grunt.file.readJSON('package.json'),

		less: {
			'build': {
				options: {
					// sourceMap: true,
					// sourceMapBasepath: 'web',
					// sourceMapRootpath: '',
					// sourceMapURL: 'main.css.map',
					// compress: true,
				},
				files: {
					'build/css/Pokerole.css': 'css/Pokerole.less',
				}
			}
		},

		clean: ['build/'],

		copy: {
			'build': {
				files: [
					{
						expand: true,
						src: [
							"system.json",
							"template.json",
							"templates/**",
							"lang/**",
							//"**/*.ts"//for source map debugging
						],
						dest: 'build/',
					},
					{
						expand: true,
						src: [
							"../SourceMaterial/*.svg"
						],
						flatten: true,
						dest: 'build/templates/actor/parts/',
					},
				]
				// rename: function (dest, src) {
				// 	//move the svgs 
				// }
			},
		},


		_watch: {
			'buildSystem': {
				files: ['Gruntfile.js', 'tsconfig.json'],
				options: {reload: true},
			},
			// 'staticRes': {
			// 	options: {atBegin: true,},
			// 	files: ...,
			// 	tasks: ['copy:build'],
			// },
			'styles': {
				options: {atBegin: true,},
				files: [
					'css/**.less',
					'css/**/**.less',
				],
				tasks: ['less'],
			},
		},

		typeScript: {},
		typeScriptWatch: {},

		concurrent: {
			'build': [
				'less',
				'copy',
				'typeScript',
			],
			// 'debug': {

			// },
			'watch': {
				tasks: [
					'_watch',
					'typeScriptWatch',
				],
				options: {
					logConcurrentOutput: true,
				}
			}
		}
	});

	grunt.loadNpmTasks('grunt-concurrent');
	grunt.loadNpmTasks('grunt-contrib-clean');
	grunt.loadNpmTasks('grunt-contrib-copy');
	grunt.loadNpmTasks('grunt-contrib-less');
	grunt.loadNpmTasks('grunt-contrib-watch');
	grunt.loadNpmTasks('grunt-contrib-uglify');

	grunt.renameTask('watch', '_watch');

	grunt.registerTask(`typeScript`, `Compile TypeScript`, getShellTask(
		"tsc", ['--pretty']
	));
	grunt.registerTask(`typeScriptWatch`, `Compile TypeScript, watch changes`, getShellTask(
		"tsc", ["-w", "--preserveWatchOutput", '--pretty']
	));

	grunt.registerTask('watch', ['concurrent:watch']);
	grunt.registerTask('default', ['concurrent:build']);
	grunt.registerTask('build', ['default']);//because I keep trying to run "grunt build"
};