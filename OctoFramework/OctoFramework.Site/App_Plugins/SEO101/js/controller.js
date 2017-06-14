angular.module('umbraco')
    .filter('ellipsisLimit',
        function() {
            return function(value, max) {
                if (!value) {
                    return '';
                }

                max = parseInt(max, 10);
                if (!max) {
                    return value;
                }

                if (value.length <= max) {
                    return value;
                }

                value = value.substr(0, max);
                return value + ' …';
            };
        });
angular.module('umbraco')
    .directive('maxlen',
        function() {
            return {
                require: 'ngModel',
                link: function(scope, el, attrs, ctrl) {

                    var validate = false;
                    var length = 999999;

                    if (attrs.name === 'title') {
                        validate = scope.model.config.allowLongTitles !== '1';
                        length = scope.serpTitleLength + scope.siteTitle.length;
                    } else if (attrs.name === 'description') {
                        validate = scope.model.config.allowLongDescriptions !== '1';
                        length = scope.serpDescriptionLength;
                    }

                    ctrl.$parsers.unshift(function(viewValue) {
                        if (validate && viewValue.length > length) {
                            ctrl.$setValidity('maxlen', false);
                        } else {
                            ctrl.$setValidity('maxlen', true);
                        }

                        return viewValue;
                    });
                }
            };
        });
angular.module('umbraco')
    .directive('minlen',
        function() {
            return {
                require: 'ngModel',
                link: function(scope, el, attrs, ctrl) {
                    var length = 999999;
                    if (attrs.name === 'description') {
                        length = scope.serpDescriptionMinimum;
                    } else if (attrs.name === 'title') {
                        length = scope.serpTitleMinimum - scope.siteTitle.length;
                    }
                    ctrl.$parsers.unshift(function(viewValue) {
                        if (viewValue.length < length && viewValue.length > 0) {
                            ctrl.$setValidity('minlen', false);
                        } else {
                            ctrl.$setValidity('minlen', true);
                        }
                        return viewValue;
                    });
                }
            };
        });
angular.module("umbraco")
    .controller("SEO101Controller",
    [
        '$scope', 'contentResource', 'editorState',
        function($scope, contentResource, editorState) {

            // default model.value
            if (!$scope.model.value) {
                $scope.model.value = {
                    title: '',
                    description: '',
                    urlName: '',
                    index: '',
                    sitemap: { priority: '', frequency: '', hide: false },
                    tags: '',
                    url: ''
                };
            }
            $scope.invalidate = true;
            $scope.model.hideLabel = true;
            $scope.serpTitleLength = !!$scope.model.config.serpTitleLength ? $scope.model.config.serpTitleLength : 65;
            $scope.serpTitleMinimum = !!$scope.model.config
                .serpTitleMinimum
                ? $scope.model.config.serpTitleMinimum
                : 10;
            $scope.serpDescriptionLength = !!$scope.model.config.serpDescriptionLength
                ? $scope.model.config.serpDescriptionLength
                : 150;
            $scope.serpDescriptionMinimum = !!$scope.model.config.serpDescriptionMinimum
                ? $scope.model.config.serpDescriptionMinimum
                : 50;
            $scope.developerName = $scope.model.config.developerName || 'your agency';
            $scope.siteTitle = $scope.model.config.siteTitle || '';
            $scope.analyzeResults = null;
            $scope.filter = null;
            $scope.loading = true;
            $scope.tagToAdd = "";
            $scope.siteTitleLength = $scope.siteTitle.length;
            $scope.listValues = [
                {
                    name: "Index this, and follow links",
                    value: "index,follow,noodp"
                },
                {
                    name: "Dont index this, but follow links",
                    value: "noindex,follow,noodp"
                },
                {
                    name: "Index none",
                    description: "Will disallow indexing of this page, and disallow it to follow links",
                    value: "noindex,nofollow,noodp"
                }
            ];
            $scope.changefreq = [
                {
                    name: "Always",
                    value: "always"

                },
                {
                    name: "Hourly",
                    value: "hourly"
                },
                {
                    name: "Daily",
                    value: "daily"
                },
                {
                    name: "Weekly",
                    value: "weekly"

                },
                {
                    name: "Monthly",
                    value: "monthly"

                },
                {
                    name: "Yearly",
                    value: "yearly"

                },
                {
                    name: "Never",
                    value: "never"

                }
            ];
            $scope.priority = [
                {
                    value: "0.0"
                },
                {
                    value: "0.1"
                },
                {
                    value: "0.2"
                },
                {
                    value: "0.3"
                },
                {
                    value: "0.4"
                },
                {
                    value: "0.5"
                },
                {
                    value: "0.6"
                },
                {
                    value: "0.7"
                },
                {
                    value: "0.8"
                },
                {
                    value: "0.9"
                },
                {
                    value: "1.0"
                }
            ];

            if (editorState.current.urls[0] !== 'This item is not published') {
                $scope.model.value.url = editorState.current.urls[0];
            } else {
                $scope.model.value.url = '';
            }

            // a very basic slugify function to replace chars in url
            function slugify(text) {
                return text.toString()
                    .toLowerCase()
                    .trim()
                    .replace(/\s+/g, '-') // Replace spaces with -
                    .replace(/&/g, '') // Replace & with nothing
                    .replace(/[^\w\-]+/g, '') // Remove all non-word chars
                    .replace(/\-\-+/g, '-'); // Replace multiple - with single -
            }

            $scope.init = function() {
                var content = $scope.GetParentContent();
                if (!content.published) {
                    // get the URL of the parent document for later
                    contentResource.getById(content.parentId)
                        .then(function(data) {
                            $scope.parentUrl = data.urls[0];
                        });
                }
                $scope.loading = false;
            };
            $scope.GetUrl = function() {
                var name, url;
                var pageContent = $scope.GetParentContent();

                // handle instances where the document is newly created and unpublished
                if (!pageContent.published) {
                    if ($scope.model.value.urlName && $scope.model.value.urlName.length) {
                        name = slugify($scope.model.value.urlName);
                    } else if (typeof(pageContent.name) !== 'undefined') {
                        name = slugify(pageContent.name);
                    }

                    if (typeof(name) === 'undefined' || name === null || name.length === 0) {
                        name = "unpublished-page";
                    }

                    return $scope.ProtocolAndHost() + $scope.parentUrl + name + "/";
                }

                var nodeUrl = pageContent.urls[0];

                // test if it is an absolute url: http://stackoverflow.com/a/10687158
                url = /^https?:\/\//i.test(nodeUrl) ? nodeUrl : $scope.ProtocolAndHost() + nodeUrl;

                var urlSplit = url.split('/');

                // we got a slug configured?
                if ($scope.model.value.urlName && $scope.model.value.urlName.length) {

                    // http://mydomain.com will be split into an array of 3 strings: 'http:', '' and 'mydomain.com'
                    // only handle slugify of urlName under root level
                    if (urlSplit.length > 4) {
                        if (urlSplit[urlSplit.length - 1] === "") {
                            urlSplit[urlSplit.length - 2] = slugify($scope.model.value.urlName);
                        } else {
                            urlSplit[urlSplit.length - 1] = slugify($scope.model.value.urlName);
                        }
                    }

                    // join new values
                    url = urlSplit.join('/');
                }
                return url;
            };
            $scope.ProtocolAndHost = function() {

                var http = location.protocol;
                var slashes = http.concat("//");
                return slashes.concat(window.location.hostname);
            };
            $scope.GetTitle = function() {
                var title = $scope.model.value.title + ' ' || 'Page Title ';
                title += $scope.siteTitle.replace('&nbsp;', ' ');
                return title;
            };
            $scope.GetParentContent = function() {
                var currentScope = $scope.$parent;
                for (var i = 0; i < 150; i++) {
                    if (currentScope && currentScope.content) {
                        return currentScope.content;
                    }
                    currentScope = currentScope.$parent;
                }
                return null;
            };


            /*if (typeof ($scope.model.value.tags) !== 'undefined' && $scope.model.value.tags.length > 0) {
                $scope.model.value.tags = $scope.model.value.tags.split(",");
            } else {
                $scope.model.value.tags = [];
            }*/

            $scope.setFilter = function(filter) {
                $scope.filter = filter;
            };

            $scope.init();
        }
    ]);
