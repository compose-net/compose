# Development
## Versioning

Every project, team, developer has their own way of versioning software, we have decided to follow [Semantic Versioning] along with common [software release life cycle] naming, using the format Major.Minor.Patch.Build-Release.

### Incrementing
Version numbers will then be incremented with the following rules:
- Major : Breaking changes to public classes/interfaces (eg. making a change to the signature of a method in application extensions)
- Minor : Completed milestone
- Patch : Completed issue
- Build : Each build through CI pipeline
- Release
  - alpha : unstable, pre-release, breaking changes can be made, features still being worked on
  - beta : features are defined and working, bugs are likely to exist and testing needs to be done, no emphasis on performance, may have pre-release dependencies
  - rc : ready for release, this is likely to go out unless significant bugs are found
  - The final released product will have no release name, just the version number

### Examples
 - 1.0.0.0
 - 1.2.3.4-alpha
 - 2.2.0.1-rc

##Project structure

We will be following the [project structure] suggested by David Fowler for .Net projects. 

## Documentation

It is expected that documentation, samples and tests are updated and with each commit/pull request as required, as opposed to being completed in bulk as a separate issue (or documentation week).

##Roadmap

These are the issues we are looking to complete in the next few versions, this is liable to change, and the most up-to-date place to check for what is currently being worked on is [Github Issues].

- v.0.1.0 - Current release, feature freeze, only resolving bugs
- v.0.2.0 - [Issue 10] IEnumerable Transitioning
- v.0.3.0 - [Issue 18] Compose.Networking

[Semantic Versioning]: http://semver.org/
[software release life cycle]: http://en.wikipedia.org/wiki/Software_release_life_cycle
[project structure]: https://gist.github.com/davidfowl/ed7564297c61fe9ab814
[Github Issues]: https://github.com/Smudge202/Compose/issues
[Issue 10]: https://github.com/Smudge202/Compose/issues/10
[Issue 18]: https://github.com/Smudge202/Compose/issues/18
