(function () {
    'use strict';
    var app = angular.module('myApp', []);
   
    //RECIPE CONTROLLER
    app.controller('Recipes', function ($scope, $http) {

        $http.get("/Recipes/GetData")
            .then(function (response) {
                // First function handles success
                $scope.Recipes = response.data;

            }, function (response) {
                // Second function handles error
                $scope.Recipes = "Something went wrong";

            });

        //Redrect index form to edit form with parameter
        $scope.RedirectToEdit = function (recipes) {

            window.location.href = '/Recipes/Edit/' + recipes.recipe_Id;
        };

        //Redrect index form to delete form with parameter
        $scope.DelRecipe = function (recipes) {

            window.location.href = '/Recipes/Delete/' + recipes.recipe_Id;
        };
        //Redrect recipe ingredients form 
        $scope.RedirectToRecipeIngredients = function (recipes) {
            
            window.location.href = '/Recipe_Ingredients/Index/?recipeID=' + recipes.recipe_Id;
            
        };
        //Redrect index form to details form with parameter
        $scope.RedirectToDetails = function (recipes) {

            window.location.href = '/Recipes/Details/' + recipes.recipe_Id;
        };
        app.filter('YesNo', function () {
            return function (text) {
                return text ? "Yes" : "No";
            }
        })
        //SORTING ICON CONTROL
        $scope.sort = {
            active: '',
            descending: undefined
        }

        $scope.changeSorting = function (column) {

            var sort = $scope.sort;

            if (sort.active == column) {
                sort.descending = !sort.descending;

            } else {
                sort.active = column;
                sort.descending = false;
            }
        };

        $scope.getIcon = function (column) {

            var sort = $scope.sort;

            if (sort.active == column) {
                return sort.descending
                    ? 'fa fa-caret-up'
                    : 'fa fa-caret-down';
            }

            return 'fa fa-caret-left';
        }

    });


    //RECIPE_INGREDIENTS CONTROLLER
    app.controller('RecipeIngredients', function ($scope, $http) {
       
        $scope.$watch('Recipe_Id', function () {

            $http.get("/Recipe_Ingredients/GetData?recipe_Id=" + $scope.Recipe_Id)
                .then(function (response) {
                    // First function handles success

                    $scope.Ingredients = response.data;

                }, function (response) {
                    // Second function handles error
                        $scope.Ingredients = "Something went wrong";

                });
        });      
        //UPDATE Amount g
        $scope.RedirectToUpdate = function (Data) {
            $scope.resultMessage = '';
            
            $http({
                url: "/Recipe_Ingredients/UpdateFromAngularController",
                contentType: 'application/json',
                method: 'POST',
                traditional: true,
                data: Data,

            }).then(function (response) {

                $scope.resultMessage = "Update Successful";
            })
                .catch(function (error) {

                    $scope.resultMessage = "Error saving";
                });
        };
        //SORTING ICON CONTROL
        $scope.sort = {
            active: '',
            descending: undefined
        }

        $scope.changeSorting = function (column) {

            var sort = $scope.sort;

            if (sort.active == column) {
                sort.descending = !sort.descending;

            } else {
                sort.active = column;
                sort.descending = false;
            }
        };

        $scope.getIcon = function (column) {

            var sort = $scope.sort;

            if (sort.active == column) {
                return sort.descending
                    ? 'fa fa-caret-up'
                    : 'fa fa-caret-down';
            }

            return 'fa fa-caret-left';
        }
        //Redrect index form to edit form with parameter
        $scope.RedirectToEdit = function (Ingredients) {

            window.location.href = '/Recipe_Ingredients/Edit/?id=' + Ingredients.id;
        };

        //Redrect index form to delete form with parameter
        $scope.DelIngredient = function (Ingredients) {

            window.location.href = '/Recipe_Ingredients/Delete/?id=' + Ingredients.id;
        };
       
        //Redrect index form to details form with parameter
        $scope.RedirectToDetails = function (Ingredients) {

        
            window.location.href = '/Recipe_Ingredients/Details/?id=' + Ingredients.id;
        };
        //Redrect recipe ingredients form 
        $scope.RedirectToIngredientNutrition = function (Ingredients) {
            
            window.location.href = '/Ingredient_Nutrients/Index/?IngredID=' + Ingredients.ingredient_Id;

        };
        app.filter('YesNo', function () {
            return function (text) {
                return text ? "Yes" : "No";
            }
        })


    });

    //INGREDIENTS CONTROLLER
    app.controller('Ingredients', function ($scope, $http) {
        $http.get("/Ingredients/GetData/")
            .then(function (response) {
                // First function handles success
                $scope.Ingredients = response.data;

            }, function (response) {
                // Second function handles error
                $scope.Ingredients = "Something went wrong";

            });
        //SORTING ICON CONTROL
        $scope.sort = {
            active: '',
            descending: undefined
        }

        $scope.changeSorting = function (column) {

            var sort = $scope.sort;

            if (sort.active == column) {
                sort.descending = !sort.descending;

            } else {
                sort.active = column;
                sort.descending = false;
            }
        };

        $scope.getIcon = function (column) {

            var sort = $scope.sort;

            if (sort.active == column) {
                return sort.descending
                    ? 'fa fa-caret-up'
                    : 'fa fa-caret-down';
            }

            return 'fa fa-caret-left';
        }
        //Redrect index form to edit form with parameter
        $scope.RedirectToEdit = function (Ingredients) {

            window.location.href = '/Ingredients/Edit/?id=' + Ingredients.ingredient_Id;
        };

        //Redrect index form to delete form with parameter
        $scope.DelIngredient = function (Ingredients) {

            window.location.href = '/Ingredients/Delete/?id=' + Ingredients.ingredient_Id;
        };
        //Redrect recipe ingredients form 
        $scope.RedirectToIngredientNutrition = function (Ingredients) {

            window.location.href = '/Ingredient_Nutrients/Index/?IngredID=' + Ingredients.ingredient_Id;

        };
        //Redrect recipe ingredients form 
        $scope.RedirectToIngredientAllergen = function (Ingredients) {

            window.location.href = '/Ingredient_Components/Index/?IngredID=' + Ingredients.ingredient_Id;

        };
        //Redrect index form to details form with parameter
        $scope.RedirectToDetails = function (Ingredients) {

            window.location.href = '/Ingredients/Details/?id=' + Ingredients.ingredient_Id;
        };
        app.filter('YesNo', function () {
            return function (text) {
                return text ? "Yes" : "No";
            }
        })


    });

    //INGREDIENT_NUTRIENT CONTROLLER
    app.controller('IngredientNutrient', function ($scope, $http) {
        
        $scope.$watch('Ingred_Id', function () {
            
            $http.get("/Ingredient_Nutrients/GetData?ingredId=" + $scope.Ingred_Id)
                .then(function (response) {
                    // First function handles success
                    //debugger;
                    $scope.IngredientNutrition = response.data;

                }, function (response) {
                    // Second function handles error
                    $scope.IngredientNutrition = "Something went wrong";

               });
        });

        
        //UPDATE NUT PER 100 GRAMS AMOUNTS
        $scope.RedirectToUpdate = function (Data) { 
            
            $scope.resultMessage = '';
            $http({
                url: "/Ingredient_Nutrients/UpdateFromAngularController",
                contentType: 'application/json',
                method: 'POST',
                traditional: true,
                data: Data,
                
            }).then(function (response) {
                
                $scope.resultMessage = "Update Successful";
            })
                .catch(function (error) {
                    
                    $scope.resultMessage = "Error saving";
                });
        };

        //SORTING ICON CONTROL
        $scope.sort = {
            active: '',
            descending: undefined
        }

        $scope.changeSorting = function (column) {

            var sort = $scope.sort;

            if (sort.active == column) {
                sort.descending = !sort.descending;

            } else {
                sort.active = column;
                sort.descending = false;
            }
        };

        $scope.getIcon = function (column) {

            var sort = $scope.sort;

            if (sort.active == column) {
                return sort.descending
                    ? 'fa fa-caret-up'
                    : 'fa fa-caret-down';
            }

            return 'fa fa-caret-left';
        }
        //Redrect index form to edit form with parameter
        $scope.RedirectToEdit = function (IngredientNutrition) {

            window.location.href = '/Ingredient_Nutrients/Edit/?id=' + IngredientNutrition.id;
        };

        //Redrect index form to delete form with parameter
        $scope.DelIngredientNutrient = function (IngredientNutrition) {

            window.location.href = '/Ingredient_Nutrients/Delete/?id=' + IngredientNutrition.id;
        };

        //Redrect index form to details form with parameter
        $scope.RedirectToDetails = function (IngredientNutrition) {

            window.location.href = '/Ingredient_Nutrients/Details/?id=' + IngredientNutrition.id;
        };



    });

    //INGREDIENT_ALLERGEN(COMPONENT) CONTROLLER
    app.controller('IngredientAllergen', function ($scope, $http) {
        
        $scope.$watch('Ingred_Id', function () {

            $http.get("/Ingredient_Components/GetData?ingredId=" + $scope.Ingred_Id)
                .then(function (response) {
                    // First function handles success

                    $scope.IngredientComponents = response.data;

                }, function (response) {
                    // Second function handles error
                        $scope.IngredientComponents = "Something went wrong";

                });
        }); 
        //SORTING ICON CONTROL
        $scope.sort = {
            active: '',
            descending: undefined
        }

        $scope.changeSorting = function (column) {

            var sort = $scope.sort;

            if (sort.active == column) {
                sort.descending = !sort.descending;

            } else {
                sort.active = column;
                sort.descending = false;
            }
        };

        $scope.getIcon = function (column) {

            var sort = $scope.sort;

            if (sort.active == column) {
                return sort.descending
                    ? 'fa fa-caret-up'
                    : 'fa fa-caret-down';
            }

            return 'fa fa-caret-left';
        }
        //Redrect index form to edit form with parameter
        $scope.RedirectToEdit = function (IngredientComponents) {

            window.location.href = '/Ingredient_Components/Edit/?id=' + IngredientComponents.id;
        };

        //Redrect index form to delete form with parameter
        $scope.DelIngredientComponent = function (IngredientComponents) {

            window.location.href = '/Ingredient_Components/Delete/?id=' + IngredientComponents.id;
        };

        //Redrect index form to details form with parameter
        $scope.RedirectToDetails = function (IngredientComponents) {

            window.location.href = '/Ingredient_Components/Details/?id=' + IngredientComponents.id;
        };



    });

    //NUTRITION CONTROLLER
    app.controller('Nutrition', function ($scope, $http) {
        $http.get("/Nutritions/GetData")
            .then(function (response) {
                // First function handles success
                $scope.Nutrients = response.data;
               
            }, function (response) {
                // Second function handles error
                $scope.Nutrients = "Something went wrong";

            });
        //SORTING ICON CONTROL
        $scope.sort = {
            active: '',
            descending: undefined
        }

        $scope.changeSorting = function (column) {

            var sort = $scope.sort;

            if (sort.active == column) {
                sort.descending = !sort.descending;

            } else {
                sort.active = column;
                sort.descending = false;
            }
        };

        $scope.getIcon = function (column) {

            var sort = $scope.sort;

            if (sort.active == column) {
                return sort.descending
                    ? 'fa fa-caret-up'
                    : 'fa fa-caret-down';
            }

            return 'fa fa-caret-left';
        }
        //Redrect index form to edit form with parameter
        $scope.RedirectToEdit = function (Nutrients) {

            window.location.href = '/Nutritions/Edit/?id=' + Nutrients.nutrition_Item_Id;
        };

        //Redrect index form to delete form with parameter
        $scope.DelNutrient = function (Nutrients) {

            window.location.href = '/Nutritions/Delete/?id=' + Nutrients.nutrition_Item_Id;
        };
        
        //Redrect index form to details form with parameter
        $scope.RedirectToDetails = function (Nutrients) {

            window.location.href = '/Nutritions/Details/?id=' + Nutrients.nutrition_Item_Id;
        };
        app.filter('YesNo', function () {
            return function (text) {
                return text ? "Yes" : "No";
            }
        })


    });

    //MEAL_CATEGORY CONTROLLER
    app.controller('MealCategory', function ($scope, $http) {
        $http.get("/Meal_Category/GetData")
            .then(function (response) {
                // First function handles success
                $scope.MealCategories = response.data;

            }, function (response) {
                // Second function handles error
                    $scope.MealCategories = "Something went wrong";

            });
        //SORTING ICON CONTROL
        $scope.sort = {
            active: '',
            descending: undefined
        }

        $scope.changeSorting = function (column) {

            var sort = $scope.sort;

            if (sort.active == column) {
                sort.descending = !sort.descending;

            } else {
                sort.active = column;
                sort.descending = false;
            }
        };

        $scope.getIcon = function (column) {

            var sort = $scope.sort;

            if (sort.active == column) {
                return sort.descending
                    ? 'fa fa-caret-up'
                    : 'fa fa-caret-down';
            }

            return 'fa fa-caret-left';
        }
        //Redrect index form to edit form with parameter
        $scope.RedirectToEdit = function (MealCategories) {
         
            window.location.href = '/Meal_Category/Edit/?id=' + MealCategories.category_Id;
        };

        //Redrect index form to delete form with parameter
        $scope.DelMealCategory = function (MealCategories) {

            window.location.href = '/Meal_Category/Delete/?id=' + MealCategories.category_Id;
        };

        //Redrect index form to details form with parameter
        $scope.RedirectToDetails = function (MealCategories) {

            window.location.href = '/Meal_Category/Details/?id=' + MealCategories.category_Id;
        };



    });

    //ALLERGEN(COMPONENT) CONTROLLER
    app.controller('Allergen', function ($scope, $http) {
        $http.get("/Components/GetData")
            .then(function (response) {
                // First function handles success
                $scope.Allergens = response.data;
                
            }, function (response) {
                // Second function handles error
                $scope.Allergens = "Something went wrong";

            });
        //SORTING ICON CONTROL
        $scope.sort = {
            active: '',
            descending: undefined
        }

        $scope.changeSorting = function (column) {

            var sort = $scope.sort;

            if (sort.active == column) {
                sort.descending = !sort.descending;

            } else {
                sort.active = column;
                sort.descending = false;
            }
        };

        $scope.getIcon = function (column) {

            var sort = $scope.sort;

            if (sort.active == column) {
                return sort.descending
                    ? 'fa fa-caret-up'
                    : 'fa fa-caret-down';
            }

            return 'fa fa-caret-left';
        }
        //Redrect index form to edit form with parameter
        $scope.RedirectToEdit = function (Allergens) {

            window.location.href = '/Components/Edit/?id=' + Allergens.comp_Id;
        };

        //Redrect index form to delete form with parameter
        $scope.DelAllergen = function (Allergens) {

            window.location.href = '/Components/Delete/?id=' + Allergens.comp_Id;
        };

        //Redrect index form to details form with parameter
        $scope.RedirectToDetails = function (Allergens) {

            window.location.href = '/Components/Details/?id=' + Allergens.comp_Id;
        };



    });

    //SERVING SIZE CONTROLLER
    app.controller('ServingSize', function ($scope, $http) {
        $http.get("/Serving_Sizes/GetData")
            .then(function (response) {
                // First function handles success
                $scope.Servings = response.data;

            }, function (response) {
                // Second function handles error
                    $scope.Servings = "Something went wrong";

            });
        //SORTING ICON CONTROL
        $scope.sort = {
            active: '',
            descending: undefined
        }

        $scope.changeSorting = function (column) {

            var sort = $scope.sort;

            if (sort.active == column) {
                sort.descending = !sort.descending;

            } else {
                sort.active = column;
                sort.descending = false;
            }
        };

        $scope.getIcon = function (column) {

            var sort = $scope.sort;

            if (sort.active == column) {
                return sort.descending
                    ? 'fa fa-caret-up'
                    : 'fa fa-caret-down';
            }

            return 'fa fa-caret-left';
        }
        //Redrect index form to edit form with parameter
        $scope.RedirectToEdit = function (Servings) {

            window.location.href = '/Serving_Sizes/Edit/?id=' + Servings.sS_Id;
        };

        //Redrect index form to delete form with parameter
        $scope.DelServingSize = function (Servings) {

            window.location.href = '/Serving_Sizes/Delete/?id=' + Servings.sS_Id;
        };

        //Redrect index form to details form with parameter
        $scope.RedirectToDetails = function (Servings) {

            window.location.href = '/Serving_Sizes/Details/?id=' + Servings.sS_Id;
        };



    });

    //VENDORS CONTROLLER
    app.controller('Vendors', function ($scope, $http) {
        $http.get("/Vendors/GetData")
            .then(function (response) {
                // First function handles success
                $scope.Vendors = response.data;

            }, function (response) {
                // Second function handles error
                $scope.Vendors = "Something went wrong";

            });
        //SORTING ICON CONTROL
        $scope.sort = {
            active: '',
            descending: undefined
        }

        $scope.changeSorting = function (column) {

            var sort = $scope.sort;

            if (sort.active == column) {
                sort.descending = !sort.descending;

            } else {
                sort.active = column;
                sort.descending = false;
            }
        };

        $scope.getIcon = function (column) {

            var sort = $scope.sort;

            if (sort.active == column) {
                return sort.descending
                    ? 'fa fa-caret-up'
                    : 'fa fa-caret-down';
            }

            return 'fa fa-caret-left';
        }
        //Redrect index form to edit form with parameter
        $scope.RedirectToEdit = function (vendor) {

            window.location.href = '/Vendors/Edit/' + vendor.vendor_Id;
        };

        //Redrect index form to delete form with parameter
        $scope.DelVendor = function (vendor) {

            window.location.href = '/Vendors/Delete/' + vendor.vendor_Id;
        };

        //Redrect index form to details form with parameter
        $scope.RedirectToDetails = function (vendor) {

            window.location.href = '/Vendors/Details/' + vendor.vendor_Id;
        };



    });
})();
