  Feature: Wallet Balance
  As an API client
  I want to request the balance from my online wallet account
  So that I can retrieve the current balance

  Background:
     Given the API endpoint to get balance is 'balance'

  Scenario: Successfully retrieve balance   
    When I send a GET request to 'get the balance'
    Then the response status should be 'Completed'
    And the response status code should be 200
    And the response status code should be OK
    And  the response should contain the balance amount '0'

  Scenario: Correct display of Balance after Deposit
    Given the user has previus deposit of '100'
    When I send a GET request to 'get the balance'
    Then the response status should be 'Completed'
    And the response status code should be 200
    And the response status code should be OK
    And  the response should contain the balance amount '100'

  Scenario: Successfully retrieve a negative balance for a user
    Given the user has negative balance of '-50'
    When I send a GET request to 'get the balance'
    Then the response status should be 'Completed'
    And the response status code should be 200
    And the response status code should be OK
    And  the response should contain the balance amount '-50'