

#     #                                           #####                #######                      
#     # #    # # ######  ####  #####  #    #     #     # ######  ####     #     ####   ####  #      
#     # ##   # # #      #    # #    # ##  ##     #       #      #    #    #    #    # #    # #      
#     # # #  # # #####  #    # #    # # ## #      #####  #####  #    #    #    #    # #    # #      
#     # #  # # # #      #    # #####  #    # ###       # #      #    #    #    #    # #    # #      
#     # #   ## # #      #    # #   #  #    # ### #     # #      #    #    #    #    # #    # #      
 #####  #    # # #       ####  #    # #    # ###  #####  ######  ####     #     ####   ####  ###### 


-----Configuration-----

Hide Dashboard
-----------------------
The performance dashboard will no longer be visable


Allow long titles
-----------------------
If ticked, long titles will not invalidate the property editor.


Allow long descriptions
-----------------------

If ticked, long descriptions will not invalidate the property editor.


SERP Title Length
-----------------------

The maximum length of a title. This isn't an exact number, so your mileage may vary. The default value of 65 is a conservative value that should work for most cases. Google will truncate overly long titles with ellipses (…)


SERP Description Length
-----------------------

The maximum length of the description. This isn't an exact number, so your mileage may vary. The default value of 150 is a conservative value that should work for most cases. Google will truncate overly long descriptions with ellipses (…)


Developer Name
-----------------------

Allows you to personalise the template a bit by putting the name of your company/agency/other. This is used within the descriptions on the view and is displayed to your content editors.


Site Title
-----------------------

Allows you to configure a site title that is appended to the title (e.g. " | My Company")


Show result types?
-----------------------

Which results should be shown


Show analyzer types?
-----------------------

Which analyzers should be shown


Using the URL Name
-----------------------

SEO101 also installs a UrlSegmentProvider to ensure the URL Name property works as intended. By default, it expects your SEO101 property to be called metadata. You can configure this property by adding the following setting to your appSettings in your web.config

<add key="SEO101.PropertyName" value="seoMetadata" />

If you want to disable the SeoMetaDataUrlSegmentProvider altogether (to add manually, or implement yourself), you can set the following appSetting to disable it.

<add key="SEO101.NoSegmentProvider" value="true" />
