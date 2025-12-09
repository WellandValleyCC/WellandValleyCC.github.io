# How to set up a new season
This guide will walk you through the steps necessary to set up a new season. 
## Prerequisites
- Draft calendar for the new season.
	- This will usually be available from the TT committee, perhaps via email.
- The private repository for the club membership data
	- Clone this from https://github.com/WellandValleyCC/WellandValleyCC.club-membership-private - branch "main"
	- Let's assume it's been cloned to: "C:\repos\wvcc\WellandValleyCC.club-membership-private
- The public repository for the club website
	- Clone this from https://github.com/WellandValleyCC/WellandValleyCC.github.io - branch "master"
	- Let's assume it's been cloned to: "C:\repos\wvcc\WellandValleyCC.github.io"
	- Note that this repository maintains two active branches
		- "master" - this is the code behind processing events and generating the live website 
		- "gh-pages" - this is the live website itself, hosted by github pages
## Required Data Files
- Previous season's membership Excel workbook
	- This is usually stored in the private membership repository.
	- e.g. "C:\repos\wvcc\WellandValleyCC.club-membership-private\data\25.01.14 2025 membership list.xlsx"
- Previous season's events data Excel workbook
	- This will be available in this repository, the public github io repository.
	- e.g. "C:\repos\wvcc\WellandValleyCC.github.io\data\ClubEvents_2025.xlsx"
## Steps to Set Up a New Season
### 1: Create New Membership Workbook
1. Create a new feature branch in the private membership repository for the new season.  e.g. "feature/2026-season-setup"
2. Copy/rename the previous season's membership workbook to reflect the new season.
	- e.g. Copy "25.01.14 2025 membership list.xlsx" to "dummy 2026 membership list.xlsx"
	- The club membership secretary will supply the correct file in January of the coming year, well before the season starts.
	- Place the new workbook in the same directory as the previous one.
	- It will be adequate to leave the data as it is for now - we just need to be able to simulate some events to be sure everything is set up correctly.
3. Edit "C:\repos\wvcc\WellandValleyCC.club-membership-private\data\competitors.meta.json" to reference 
the new dummy membership file, with "use_simulated_import_date" set to false.
	- There is no need to force a specific import date, as the new season will not start until 2026.
	- Sample contents of the updated "competitors.meta.json" file:

``` json
{
  "source": "dummy 2026 membership list.xlsx",
  "use_simulated_import_date": false,
  "simulated_import_date": "2025-12-09"
}
```
4. Commit and push the changes to the feature branch.
    - See example commit: https://github.com/WellandValleyCC/WellandValleyCC.club-membership-private/commit/bf9a3afb4352cae82d4da527748b80fa0521f2d8 
	- pipeline will run to push the public membership csv to the public repository.
	- e.g. https://github.com/WellandValleyCC/WellandValleyCC.club-membership-private/actions/runs/20066947127

### 2: Create New Events Workbook
