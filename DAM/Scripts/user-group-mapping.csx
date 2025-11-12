using System.Linq;
using Stylelabs.M.Sdk;
using Stylelabs.M.Scripting.Types.V1_0.User;
using Stylelabs.M.Scripting.Types.V1_0.User.SignIn;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

const string ROLES_MAP_SETTING_CATEGORY = "Security";
const string ROLES_MAP_SETTING_NAME = "SSOMapping";

class GroupMapping {
    public string label {get;set; }
    public string external {get; set;}
    public string[] internalGroups {get; set;}
}

class MappingSetting {
	public List<GroupMapping> groupMapping {get;set;}
	public string ClaimType {get;set;}
	public List<string> defaultGroups {get;set;}
}

try {
    await RunScriptAsync();
}
catch(Exception ex)
{
    MClient.Logger.Error(ex);
}

public async Task RunScriptAsync() {
	
    if(Context.AuthenticationSource == AuthenticationSource.Internal) {
        MClient.Logger.Info($"Authentication source is Internal. Exiting.");
        return;
    }

    MClient.Logger.Info($"Script called for user {Context.User.Id}");
    MClient.Logger.Debug($"User claims: {string.Join(",\r\n", Context.ExternalUserInfo.Claims.Select(c => $"{c.Type}: {c.Value}"))}");

    var roleMapSettings = await GetSettingValueAsync<JObject>(ROLES_MAP_SETTING_CATEGORY, ROLES_MAP_SETTING_NAME);
    if(roleMapSettings == null) {
        MClient.Logger.Error($"Could not load role map settings. Exiting.");
        return;
    }
    var mappingSetting = roleMapSettings.ToObject<MappingSetting>();

    var groupClaims = GetClaimValues(mappingSetting.ClaimType);
    if(!groupClaims.Any()) {
        MClient.Logger.Info($"No claims of type {mappingSetting.ClaimType}. Exiting.");
        return;
    }
    MClient.Logger.Debug($"Found {groupClaims.Count()} claim(s). {string.Join(",\r\n", groupClaims)}");

	var groupMaps = mappingSetting.groupMapping.Where(m => groupClaims.Contains(m.external));
    MClient.Logger.Info($"Found {groupMaps.Count()} group map(s). For roles\r\n{string.Join(", ", groupMaps.Select(rm => rm.external))}");

    var userGroupNames = groupMaps.SelectMany(rm => rm.internalGroups).ToList();
    MClient.Logger.Info($"Found {userGroupNames.Count()} user group(s).\r\n{string.Join(", ", userGroupNames)}");
	
	// Adding the default usergroups to the list
	userGroupNames.AddRange(mappingSetting.defaultGroups);

    var groupIds = await GetIdsForGroupNamesAsync(userGroupNames);
    await AddUserGroupsToUserAsync(groupIds.Distinct());
}

#region functions
public IEnumerable<string> GetClaimValues(string type) {
    return Context.ExternalUserInfo.Claims.Where(c => c.Type == type).Select(c => c.Value);
}

public async Task<T> GetSettingValueAsync<T>(string category, string name) {
    var settingEntity = await MClient.Settings.GetSettingAsync(category, name).ConfigureAwait(false);
    return settingEntity.GetPropertyValue<T>("M.Setting.Value");
}

public async Task<IEnumerable<long>> GetIdsForGroupNamesAsync(IEnumerable<string> groupNames) {
    var query = Query.CreateQuery(entities =>
        from e in entities
        where e.DefinitionName == "Usergroup" && e.Property("GroupName").In(groupNames)
        select e);

    var queryResult = await MClient.Querying.QueryIdsAsync(query);

    return queryResult.Items;
}

public async Task AddUserGroupsToUserAsync(IEnumerable<long> userGroupIds) {
    var userGroupToUserRelation = await Context.User.GetRelationAsync<IChildToManyParentsRelation>("UserGroupToUser");
	
	// REPLACE the usergroups as per mapping result
	// Update usergroup to user relation
	userGroupToUserRelation.SetIds(userGroupIds);
	
	// Update usergroup configuration on the user. 
	var userGroupConfigurationProperty = Context.User.GetPropertyValue<JToken>("UserGroupConfiguration");

    if(userGroupConfigurationProperty == null){
        Context.User.SetPropertyValue("UserGroupConfiguration",JToken.Parse("{\"combine_method\": \"Any\",\"user_group_ids\": [],\"children\": []}"));
        userGroupConfigurationProperty = Context.User.GetPropertyValue<JToken>("UserGroupConfiguration");
    }

    if (userGroupConfigurationProperty != null)
    {
        var jGroupsIds = JToken.FromObject(userGroupIds);
        MClient.Logger.Info($"Applying following user groups: {jGroupsIds} for user with id {Context.User.Id}");
        
        userGroupConfigurationProperty.SelectToken("user_group_ids")?.Replace(jGroupsIds);
    }
    else
    {
        MClient.Logger.Info("UserGroupConfiguration property is null somehow");
    }
	

    await MClient.Entities.SaveAsync(Context.User);
}
#endregion