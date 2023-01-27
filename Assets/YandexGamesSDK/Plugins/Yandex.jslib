mergeInto(LibraryManager.library,
{
    SavePlayerData: function(data)
    {
        var dataString = UTF8ToString(data);
        var dataObj = JSON.parse(dataString);
        player.setData(dataObj, true);
    },
    
    LoadPlayerData: function()
    {
        player.getData().then(data =>
        {
            const dataString = JSON.stringify(data);
            myGameInstance.SendMessage('Yandex', 'SetPlayerData', dataString);
        });
    },
    
    UpdateLeaderboard: function(name, value)
    {
        if (player.getMode() === 'lite')
        {
            return;
        }
    
        var nameString = UTF8ToString(name);
        leaderboards.setLeaderboardScore(nameString, value);
        console.log('setLeaderboardScore');
    },
    
    RateGame: function()
    {
        ysdk.feedback.canReview().then(({value, reason}) =>
        {
            if (value)
            {
                ysdk.feedback.requestReview();
            }
            else
            {
                console.log(reason);
            }
        });
    },

    ShowFullscreenAdv: function()
    {
        ysdk.adv.showFullscreenAdv(
        {
            callbacks: 
            {
                onClose: function(wasShown) 
                {
                    myGameInstance.SendMessage('Yandex', 'OnFullscrenAdvClosed');
                },
                
                onError: function(error) 
                {
                    myGameInstance.SendMessage('Yandex', 'OnFullscrenAdvError');
                }
            }
        });
    }
});