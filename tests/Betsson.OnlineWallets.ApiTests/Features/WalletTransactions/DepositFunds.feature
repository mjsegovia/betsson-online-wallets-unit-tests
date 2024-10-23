@deposit
Feature: Deposit Funds
  As a user or API client,
  I want to deposit funds into a online wallet account
  So that I can increase my balance and use the funds for future transactions.

  Background:
    Given the API endpoint for depositing funds is '/deposit'
    And I check my current balance before transaction

     @HappyPath
  Scenario: Successfully deposit funds into user account
    When I send a deposit POST request with the payload:
    | amount  |
    | 100.00  |
    Then the transacction should be successful with status 'Completed' 
    And the response status code should be '200'
    And the status description should be 'OK'    
    And the balance should be updated to reflect the 'deposit' transaction
    
    @ErrorResponse
  Scenario: Fail to deposit funds with an invalid amount
    When I send a deposit POST request with the payload:
    | amount | 
    | -50.00 |
    Then the response status code should be '400'
    And the status description should be 'Bad Request'
    And the response status should be 'Error'
    And the response should contain an error message ''Amount' must be greater than or equal to '0'.'
    And the balance should not be updated

    @ignore
  Scenario: Handle multiple concurrent deposits correctly    
    When I send a deposit POST request with the payload:
    | amount | 
    | 100.00 |
    Then the response status code of each request should be 200 (OK)
    And the final balance should reflect all '5' deposits

