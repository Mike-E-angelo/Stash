﻿using DragonSpark.Compose;
using DragonSpark.Model.Selection;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;

// ReSharper disable All

namespace DragonSpark.Presentation.Components.Routing
{
	/// <summary>
	/// ATTRIBUTION: https://github.com/ShaunCurtis/CEC.Routing/tree/master/CEC.Routing/Routing
	/// </summary>
	[DebuggerDisplay("Handler = {Handler}, Template = {Template}")]
	class RouteEntry : ISelect<RouteContext, RouteData?>
	{
		readonly static IReadOnlyDictionary<string, object> Empty = new Dictionary<string, object>().AsReadOnly();

		public RouteEntry(RouteTemplate template, Type handler, string[] unusedRouteParameterNames)
		{
			Template                  = template;
			UnusedRouteParameterNames = unusedRouteParameterNames;
			Handler                   = handler;
		}

		public RouteTemplate Template { get; }

		public string[] UnusedRouteParameterNames { get; }

		public Type Handler { get; }

		public RouteData? Get(RouteContext context)
		{
			// If there are no optional segments on the route and the length of the route
			// and the template do not match, then there is no chance of this matching and
			// we can bail early.
			var total    = Template.Segments.Length;
			var length   = context.Segments.Length;
			var optional = Template.OptionalSegmentsCount;
			if (optional != 0 || total == length)
			{
				Dictionary<string, object> parameters = null!;
				var                        matching   = 0;
				for (var i = 0; i < total; i++)
				{
					var segment = Template.Segments[i];

					// If the template contains more segments than the path, then
					// we may need to break out of this for-loop. This can happen
					// in one of two cases:
					//
					// (1) If we are comparing a literal route with a literal template
					// and the route is shorter than the template.
					// (2) If we are comparing a template where the last value is an optional
					// parameter that the route does not provide.
					if (i >= length && !segment.IsParameter && !segment.IsOptional)
					{
						break;
					}

					var path = i < length ? context.Segments[i] : null!;
					if (segment.Match(path, out var matched))
					{
						matching++;
						if (segment.IsParameter)
						{
							parameters                ??= new Dictionary<string, object>(StringComparer.Ordinal);
							parameters[segment.Value] =   matched;
						}
					}
					else
					{
						return null;
					}
				}

				// In addition to extracting parameter values from the URL, each route entry
				// also knows which other parameters should be supplied with null values. These
				// are parameters supplied by other route entries matching the same handler.
				var unused = UnusedRouteParameterNames.Length;
				if (unused > 0)
				{
					parameters ??= new Dictionary<string, object>(StringComparer.Ordinal);
					for (var i = 0; i < unused; i++)
					{
						parameters[UnusedRouteParameterNames[i]] = null!;
					}
				}

				// We track the number of segments in the template that matched
				// against this particular route then only select the route that
				// matches the most number of segments on the route that was passed.
				// This check is an exactness check that favors the more precise of
				// two templates in the event that the following route table exists.
				//  Route 1: /{anythingGoes}
				//  Route 2: /users/{id:int}
				// And the provided route is `/users/1`. We want to choose Route 2
				// over Route 1.
				// Furthermore, literal routes are preferred over parameterized routes.
				// If the two routes below are registered in the route table.
				// Route 1: /users/1
				// Route 2: /users/{id:int}
				// And the provided route is `/users/1`. We want to choose Route 1 over
				// Route 2.
				var allRouteSegmentsMatch = matching >= length;
				// Checking that all route segments have been matches does not suffice if we are
				// comparing literal templates with literal routes. For example, the template
				// `/this/is/a/template` and the route `/this/`. In that case, we want to ensure
				// that all non-optional segments have matched as well.
				var allNonOptionalSegmentsMatch = matching >= (total - optional);
				if (allRouteSegmentsMatch && allNonOptionalSegmentsMatch)
				{
					return new RouteData(Handler, parameters ?? Empty);
				}
			}

			return null;
		}
	}
}