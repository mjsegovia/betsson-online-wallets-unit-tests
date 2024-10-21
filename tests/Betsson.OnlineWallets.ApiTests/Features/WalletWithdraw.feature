  Feature: User API - List Users
  As an API client
  I want to request a list of users by page number
  So that I can retrieve a paginated list of users from the API

  Background:
    Given the API endpoint to list users is 'users'    

  Scenario Outline: Successfully retrieve a list of users for a specific page
    When I send a GET request with the query parameter 'page' set to '<PageNumber>'
    Then the response status should be 'Completed'
    And the response status code should be 'OK'    
    And the page number returned matches with the one specified in the URL
     And the list should contain the following user for page <PageNumber>
    |first_name |last_name |email  |avatar  |
    |<FirstName>|<LastName>|<Email>|<Avatar>|
   
   Examples: 
   | PageNumber | FirstName | LastName | Email                  | Avatar                                   |
   | 1          | Janet     | Weaver   | janet.weaver@reqres.in | https://reqres.in/img/faces/2-image.jpg  |
   | 2          | Byron     | Fields   | byron.fields@reqres.in | https://reqres.in/img/faces/10-image.jpg |

   
   Scenario: Non users are retrieve for a nonexistent page
   When I send a GET request with the query parameter 'page' set to '12'
   Then the response status code should be 'OK'    
    And the page number returned matches with the one specified in the URL
    And the response should not contain users