import { AppPage } from './app.po';

describe('workspace-project App', () => {
  let page: AppPage;
  const randomString = () =>{
    const n = Math.floor(Math.random() * 100000);
    return n.toString();
  }
  beforeEach(() => {
    page = new AppPage();
  });

  it('should display `ITI-Poll`', async () => {
    await page.navigateTo();
    expect(await page.HomeLinkText()).toEqual('ITI-Poll');
  });

  it('should sign up', async () =>{
    await page.navigateToSignUp();
    const email = `test-${randomString()}@e2e.fr`;
    const nickname = `e2e-${randomString()}`;
    const password = 'validpassword';

    await page.fillSignUpForm(email, nickname, password)
    await page.signUp();

    expect(await page.email()).toEqual(email);
  });

  it('should log in', async() =>{
    // create user
    await page.navigateToSignUp();
    const email = `test-${randomString()}@e2e.fr`;
    const nickname = `e2e-${randomString()}`;
    const password = 'validpassword';

    await page.fillSignUpForm(email, nickname, password)
    await page.signUp();

    //log in 
    await page.navigateTo();
    await page.fillLogIn(email, password);
    await page.logIn();
    expect(await page.email()).toEqual(email);

  })

});
