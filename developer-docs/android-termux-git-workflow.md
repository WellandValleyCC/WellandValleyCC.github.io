# Termux Setup & Commands Used

This document summarises all Termux commands used to configure Git, enable sparse checkout, authenticate via SSH, and create the `wvcc` automation script for updating WVCC spreadsheets from Android.

---

## 1. Initial Termux Setup

```sh
termux-setup-storage
pkg install git
```

---

## 2. Navigate to Documents Folder

```sh
cd /storage/emulated/0/Documents
ls
```

---

## 3. Clone WVCC Repo with Sparse Checkout

```sh
git clone --no-checkout https://github.com/WellandValleyCC/WellandValleyCC.github.io.git
cd WellandValleyCC.github.io

git sparse-checkout init --cone

git config --global --add safe.directory /storage/emulated/0/Documents/WellandValleyCC.github.io

git sparse-checkout set \
  data/ClubEvents_2025.xlsx \
  data/ClubEvents_2026.xlsx
```

---

## 4. Switch to Working Branch

```sh
git checkout feature/round-robin
```

---

## 5. Manual Spreadsheet Commit Workflow

```sh
git status
git add data/ClubEvents_2025.xlsx
git commit -m "Update ClubEvents spreadsheet"
```

Set Git identity:

```sh
git config --global user.name "mike-ives-uk"
git config --global user.email "mbives+wvcc@gmail.com"
```

Commit again with updated message:

```sh
git commit -m "Update ClubEvents spreadsheet from android"
git push
```

---

## 6. Switch to SSH Authentication

```sh
ssh-keygen -t ed25519 -C "mbives+wvcc@gmail.com"
cat ~/.ssh/id_ed25519.pub
git remote set-url origin git@github.com:WellandValleyCC/WellandValleyCC.github.io.git
```

---

## 7. Push Another Spreadsheet Update

```sh
git pull
git add data/ClubEvents_2025.xlsx
git commit -m "Update ClubEvents spreadsheet from android again"
git push
```

---

## 8. Create and Ignore a Temporary Update Script

```sh
nano update.sh
chmod +x update.sh
echo "update.sh" >> .gitignore
git rm --cached update.sh
git add .gitignore
git commit -m "Ignore local update script"
git push
```

---

## 9. Create the `wvcc` Automation Script

```sh
nano ~/wvcc
chmod +x ~/wvcc
ls ~
```

---

## 10. Install Script into `~/bin`

```sh
mkdir -p ~/bin
mv ~/wvcc ~/bin/wvcc
chmod +x ~/bin/wvcc
```

---

## 11. Fix PATH to Include `~/bin`

```sh
echo 'export PATH=$HOME/bin:$PATH' >> ~/.bashrc
source ~/.bashrc
echo $PATH
```

Expected PATH:

```
/data/data/com.termux/files/home/bin:/data/data/com.termux/files/usr/bin
```

---

## 12. Run the Automation Script

```sh
wvcc
wvcc feature/round-robin
```

---

## 13. Full Command History (for reference)

```
1  termux-setup-storage
2  pkg install git
3  cd /storage/emulated/0/Documents
4  ls
5  git clone --no-checkout https://github.com/WellandValleyCC/WellandValleyCC.github.io.git
6  ls
7  cd WellandValleyCC.github.io
8  git sparse-checkout init --cone
9  git config --global --add safe.directory /storage/emulated/0/Documents/WellandValleyCC.github.io
10 git sparse-checkout init --cone
11 git config --global --add safe.directory /storage/emulated/0/Documents/WellandValleyCC.github.io
12 git sparse-checkout init --cone
13 git sparse-checkout set data/ClubEvents_2025.xlsx data/ClubEvents_2026.xlsx
14 ls
15 git checkout feature/round-robin
16 ls
17 cd data
18 ls
19 git status
20 cd ..
21 git add data/ClubEvents_2025.xlsx
22 git status
23 git commit -m "Update ClubEvents spreadsheet"
24 git config --global user.name "mike-ives-uk"
25 git config --global user.email "mbives+wvcc@gmail.com"
26 git commit -m "Update ClubEvents spreadsheet from android"
27 git push
28 ssh-keygen -t ed25519 -C "mbives+wvcc@gmail.com"
29 cat ~/.ssh/id_ed25519.pub
30 cd /storage/emulated/0/Documents/WellandValleyCC.github.io
31 git remote set-url origin git@github.com:WellandValleyCC/WellandValleyCC.github.io.git
32 git pull
33 git status
34 git add data/ClubEvents_2025.xlsx
35 git commit -m "Update ClubEvents spreadsheet from android again"
36 
37 git push
38 nano update.sh
39 chmod +x update.sh
40 git status
41 echo "update.sh" >> .gitignore
42 git rm --cached update.sh
43 git status
44 git add .gitignore
45 git commit -m "Ignore local update script"
46 git push
47 nano ~/wvcc
48 ls ~
49 chmod +x ~/wvcc
50 wvcc
51 mkdir -p ~/bin
52 mv ~/wvcc ~/bin/wvcc
53 chmod +x ~/bin/wvcc
54 wvcc
55 exit
56 ls
57 echo $PATH
58 wvcc
59 wvcc feature/round-robin
60 exit
61 history
```

## 14. wvcc Automation Script

The `wvcc` script provides a one‑command workflow for updating WVCC spreadsheet files from Android using Termux. It performs branch selection, pulls the latest changes, stages spreadsheet updates, commits only when necessary, and pushes to the selected branch.

### Script Contents

```sh
#!/data/data/com.termux/files/usr/bin/bash

# Allow branch name as first argument, default to master
BRANCH="${1:-master}"

# Jump to your repo
cd /storage/emulated/0/Documents/WellandValleyCC.github.io || exit

# Switch to the requested branch
git checkout "$BRANCH"

# Pull latest changes
git pull

# Pause here so you can edit spreadsheets in Excel
read -p "Press Enter when you have finished editing spreadsheets..."

# Stage spreadsheet updates
git add data

# Commit only if there are changes
if ! git diff --cached --quiet; then
    git commit -m "Update spreadsheets from Android on $(date '+%Y-%m-%d %H:%M')"
    git push
    echo "✔ Spreadsheet changes pushed to $BRANCH"
else
    echo "✔ No changes to commit on $BRANCH"
fi
```

### Usage

Run with the default branch:

```sh
wvcc
```

Or specify a branch explicitly:

```sh
wvcc feature/round-robin
```

## 15. Why the `wvcc` Script Exists

Updating WVCC spreadsheets from Android is possible with plain Git commands, but the workflow is repetitive and error‑prone when done manually. The `wvcc` script automates the entire sequence, ensuring consistent behaviour every time it runs.

### Purpose

- Provide a **single command** to update spreadsheets from Android.
- Ensure the correct **branch is selected** before committing.
- Always **pull latest changes** to avoid conflicts.
- Stage only the relevant **`data/`** directory.
- Commit **only when changes exist**, avoiding empty commits.
- Push updates with a **timestamped message** for clear audit trails.
- Make the workflow **fast, predictable, and contributor‑proof**.

### Benefits

- Eliminates manual Git steps on a mobile keyboard.
- Prevents accidental commits to the wrong branch.
- Ensures consistent commit messages with timestamps.
- Reduces the chance of merge conflicts.
- Makes Android spreadsheet updates as simple as:

```sh
wvcc
```

or, for a specific branch:

```sh
wvcc feature/round-robin
```

### Behaviour Summary

- Defaults to `master` unless a branch name is provided.
- Navigates directly to the repo location in shared storage.
- Checks out the requested branch.
- Pulls the latest upstream changes.
- Stages all spreadsheet updates under `data/`.
- Commits only if changes are detected.
- Pushes to the same branch.
- Prints a clear success or no‑changes message.

This script turns a multi‑step Git workflow into a single, reliable command optimised for Termux on Android.