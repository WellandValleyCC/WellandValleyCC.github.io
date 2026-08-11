# How to record/publish the results of an event

## Browse to the GitHub page

![https://github.com/WellandValleyCC](..\images\how-to-score-image-0.png)

## Sign in

![sign in to GitHub](..\images\how-to-score-image-1.png)

(If you don't yet have an account, speak to Mike Ives or Jon Durnin)

![Landing page](..\images\how-to-score-image-2.png)

## Select the WVCC website repository

![Select WellandValleyCC.github.io from Repositories tab](..\images\how-to-score-image-3.png)

## Browse to the `data` folder

![`data` folder](..\images\how-to-score-image-4.png)

## Locate the season's club events Excel workbook

![ClubEvents_2026.xlsx](..\images\how-to-score-image-5.png)

This needs to be in the `master` branch.

Click on the file and then select to download it to your PC / laptop

![Download button](..\images\how-to-score-image-6.png)

![Downloaded Excel workbook](..\images\how-to-score-image-7.png)

## Edit the results sheet

You may need to enable editing.  The workbook is `.xlsx` meaning there are no macros, so there's no security concerns in doing this.

Look at RoundRobinRiders sheet and EventNN sheet.

For the event on 13th August 2026, you can see that it is event 22 by clicking through to it from the index page https://wellandvalleycc.github.io - look at the URL in the browser.

![wellandvalleycc.github.io Event 22](..\images\how-to-score-image-8.png)

### Enter the rider names in Column A.

![Excel Event Sheet and RoundRobinRiders sheet](..\images\how-to-score-image-9.png)

- For WVCC club members, you *MUST* use the name that person is registered with.  You can tell you've got this right if their club number shows in Column B.
    - E.g. Jonathan Durnin - not Jon Durnin
- For Second claim members, again you *MUST* use their registered name 
    - E.g. Jamie Kershaw
- For other riders, use the name from Column A in the RoundRobinRiders sheet - including any parenthetical first claim club name .  You can tell you've got this right if their name in Column B matches RoundRobinRiders Column C, with the Round Robin Club they represent appearing as a suffix on their name
    - E.g. "Mike Deely (G Fox)" --> "Mike Deely (G Fox) (RFW)"

Any new Round Robin riders can be added to the RoundRobinRiders sheet.  The need to do this becomes less and less frequent as time goes on.

### Set the Road Bike - TT Bike status

Enter `r` in Column F for any riders on a road bike

### Enter the riders' times 

When known, the times go in columns C, D and E.  You can publish start sheets by moving on to commit the updated workbook before the event has been ridden.

DNS, DNF and DQ go in column G when relevant.

### Save the changes to the Excel workbook

Don't forget to save your changes. 

## Commit the updated workbook to the Git repo

Back in the web browser, from where you downloaded the workbook, go back to the `data` folder on the `master` branch.  Select Upload files under the `Add file` button

![Upload files to `master` branch, `data` folder](..\images\how-to-score-image-10.png)

![Upload files page](..\images\how-to-score-image-11.png)

From this page choose the Excel workbook that you've just edited.  And enter a comment in the Commit changes controls.

![Commit the changed workbook](image.png)

- Note that the name of the file you're about to upload is now shown above the comments section
- Enter a suitable comment 
    - E.g. WVCC Event 22 - Round Robin Event 10 results
- Press the Commit changes button

## Wait a few minutes for the magic to happen

The pipelines will automatically run and create the updated websites for both the WVCC site and for the Round Robin site.

The end result here will be Pull Requests which need to be merged in order for the changes to go live.  See How-to-merge-results-PRs.md
