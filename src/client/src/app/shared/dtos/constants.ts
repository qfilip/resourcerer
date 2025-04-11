export const permissionsMap: { [key:string]: string[] } = {
	'Company': ['View','Create','Update','Remove','Delete'],
	'User': ['View','Create','Update','Remove','Delete'],
	'Category': ['View','Create','Update','Remove','Delete'],
	'Item': ['View','Create','Update','Remove','Delete'],
	'ItemEvent': ['View','Create','Update','Remove','Delete'],
	'Instance': ['View','Create','Update','Remove','Delete'],
	'InstanceEvent': ['View','Create','Update','Remove','Delete'],
};
export const jwtClaimKeys = {
	id: 'user_id',
	name: 'user_name',
	displayName: 'user_name',
	email: 'user_email',
	isAdmin: 'user_is_admin',
	companyId: 'user_company_id',
	companyName: 'user_company_name',
};
