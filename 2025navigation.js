// navigation.js
window.onload = function() {
    // Define the data table with your custom navigation order
    var pages = [
        { "name": "Event 1", "file": "2025ClubChampionshipEvent1.htm" },
        { "name": "Event 2", "file": "2025ClubChampionshipEvent2.htm" },
        { "name": "Event 3", "file": "2025ClubChampionshipEvent3.htm" },
        { "name": "Event 4", "file": "2025ClubChampionshipEvent4.htm" },
        { "name": "Event 5", "file": "2025ClubChampionshipEvent5.htm" },
        { "name": "Event 6", "file": "2025ClubChampionshipEvent6.htm" },
        { "name": "Event 7", "file": "2025ClubChampionshipEvent7.htm" },
        { "name": "Event 8", "file": "2025ClubChampionshipEvent8.htm" },
        { "name": "Event 9", "file": "2025ClubChampionshipEvent9.htm" },
        { "name": "Event 10", "file": "2025ClubChampionshipEvent10.htm" },
        { "name": "Event 11", "file": "2025ClubChampionshipEvent11.htm" },
        { "name": "Event 12", "file": "2025ClubChampionshipEvent12.htm" },
        { "name": "Event 13", "file": "2025ClubChampionshipEvent13.htm" },
        { "name": "Event 14", "file": "2025ClubChampionshipEvent14.htm" },
        { "name": "Event 15", "file": "2025ClubChampionshipEvent15.htm" },
        { "name": "Event 16", "file": "2025ClubChampionshipEvent16.htm" },
        { "name": "Event 17", "file": "2025ClubChampionshipEvent17.htm" },
        { "name": "Event 18", "file": "2025ClubChampionshipEvent18.htm" },
        { "name": "Event 19", "file": "2025ClubChampionshipEvent19.htm" },
        { "name": "Event 20", "file": "2025ClubChampionshipEvent20.htm" },
        { "name": "Event 21", "file": "2025ClubChampionshipEvent21.htm" },
        { "name": "Event 22", "file": "2025ClubChampionshipEvent22.htm" },
        { "name": "Event 23", "file": "2025ClubChampionshipEvent23.htm" },
        { "name": "Event 24", "file": "2025ClubChampionshipEvent24.htm" },
        { "name": "Event 25", "file": "2025ClubChampionshipEvent25.htm" },
        { "name": "Event 26", "file": "2025ClubChampionshipEvent26.htm" },
        { "name": "Seniors", "file": "2025ClubChampionshipSeniors.htm" },
        { "name": "Veterans", "file": "2025ClubChampionshipVeterans.htm" },
        { "name": "Women", "file": "2025ClubChampionshipWomen.htm" },
        { "name": "Juniors", "file": "2025ClubChampionshipJuniors.htm" },
        { "name": "Juveniles", "file": "2025ClubChampionshipJuveniles.htm" },
        { "name": "Road Bike Men", "file": "2025ClubChampionshipRoadBikeMen.htm" },
        { "name": "Road Bike Women", "file": "2025ClubChampionshipRoadBikeWomen.htm" },
        { "name": "Premier League", "file": "2025ClubChampionshipLeaguePrem.htm" },
        { "name": "League 1", "file": "2025ClubChampionshipLeague1.htm" },
        { "name": "League 2", "file": "2025ClubChampionshipLeague2.htm" },
        { "name": "League 3", "file": "2025ClubChampionshipLeague3.htm" },
        { "name": "League 4", "file": "2025ClubChampionshipLeague4.htm" },
        { "name": "Nev Brooks", "file": "2025ClubChampionshipNevBrooks.htm" }
    ];
    
    // Get current page file name from URL
    var currentPage = window.location.pathname.split("/").pop();
    
    // Find the index of the current page in the data table
    var currentIndex = pages.findIndex(page => page.file === currentPage);
    
    // Generate navigation links
    var navLinks = '<div class="col-sm-3" align="center">';
    
    // Calculate previous and next page indices with wrap-around logic
    var prevIndex = (currentIndex - 1 + pages.length) % pages.length;
    var nextIndex = (currentIndex + 1) % pages.length;
    
    // Add a link to the previous page
    var prevPage = pages[prevIndex];

    navLinks += '<a href="' + prevPage.file + '">' + prevPage.name + '</a> ';
    
	navLinks += '<a href="index.htm">Index</a>';
	  
    // Add a link to the next page
    var nextPage = pages[nextIndex];
    navLinks += '<a href="' + nextPage.file + '">' + nextPage.name + '</a>';
        
    // Insert navigation links into the page
    document.getElementById('nav').innerHTML = navLinks;
}
