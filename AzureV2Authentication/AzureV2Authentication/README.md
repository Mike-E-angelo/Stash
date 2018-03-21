Steps:
	1. Sign into your function at https://<function-name>.azurewebsites.net
	1. CTRL-SHIFT-C in Chrome -> Application -> Cookies -> <sitename> -> AppServiceAuthSession -> Copy Value
	1. Open local.settings.json and paste value in `AuthenticationToken` setting.
	1. While you're there, paste in the URL from first site in `AuthenticationBaseAddress`
	1. Launch application.
	1. Cross fingers.
	1. Enjoy magic (Hopefully.)